using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.WebAPI.HttpContexts;
using SilentMoonApp.WebAPI.Middlewares;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Diagnostics;
using System.Text;
using SilentMoonApp.WebAPI.Filters;

namespace SilentMoonApp.WebAPI;

public static class DependencyInjection
{
	public static IServiceCollection AddWebAPILayer(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddHttpContextAccessor();
		services.AddLocalization();

		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();


		AddServices(services);
		AddRateLimiters(services);
		AddHttpCookieSettings(services, configuration);	
		AddJwtAuthentication(services, configuration);
		AddSwaggerDocumentation(services);


		return services;
	}



	// Helpers

	private static void AddJwtAuthentication(this IServiceCollection services,
												  IConfiguration configuration)
	{
		JwtSettings jwtSettings = configuration.GetRequiredSection(JwtSettings.SectionName)
											   .Get<JwtSettings>()
								?? throw new InvalidOperationException($"{JwtSettings.SectionName} konfiqurasiyasi tapilmadi.");


		byte[] signingKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

		services.AddAuthentication(opt =>
		{
			opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer();

		services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
				.Configure<IOptions<JwtSettings>>((bearerOptions, settingsOptions) =>
				{
					JwtSettings settings = settingsOptions.Value;

					bearerOptions.MapInboundClaims = false;

					bearerOptions.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtSettings.Issuer,
						ValidAudience = jwtSettings.Audience,
						IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
						NameClaimType = JwtRegisteredClaimNames.UniqueName,
						RoleClaimType = ClaimTypes.Role,
						ClockSkew = TimeSpan.Zero
					};
				});

		services.AddAuthorization();
	}


	private static void AddRateLimiters(IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			options.OnRejected = async (context, ct) =>
			{
				string traceId = Activity.Current?.TraceId.ToString()
							  ?? context.HttpContext.TraceIdentifier;

				ProblemDetails problemDetails = new()
				{
					Type = "urn:problem-type:rate_limit.exceeded",
					Status = StatusCodes.Status429TooManyRequests,
					Title = "Too Many Requests",
					Detail = "Too many request attempts. Please try again later.",
					Instance = context.HttpContext.Request.Path
				};

				problemDetails.Extensions["code"] = "rate_limit.exceeded";
				problemDetails.Extensions["traceId"] = traceId;

				context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
				context.HttpContext.Response.ContentType = "application/problem+json";

				await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, ct);
			};

			options.AddPolicy("auth-login", httpContext =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: GetRateLimitPartitionKey(httpContext),
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 5,
						Window = TimeSpan.FromMinutes(1),
						QueueLimit = 0,
						AutoReplenishment = true
					}
				)
			);

			options.AddPolicy("auth-otp", httpContext =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: GetRateLimitPartitionKey(httpContext),
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 3,
						Window = TimeSpan.FromMinutes(10),
						QueueLimit = 0,
						AutoReplenishment = true
					}
				)
			);

			options.AddPolicy("auth-oauth", httpContext =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: GetRateLimitPartitionKey(httpContext),
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 10,
						Window = TimeSpan.FromMinutes(1),
						QueueLimit = 0,
						AutoReplenishment = true
					}
				)
			);
		});
	}


	private static string GetRateLimitPartitionKey(HttpContext httpContext)
	{
		string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
						?? "unknown";

		return $"{httpContext.Request.Path}:{ipAddress}";
	}


	private static void AddSwaggerDocumentation(this IServiceCollection services)
	{
		const string bearerScheme = "Bearer";

		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc(
			   "v1",
			   new OpenApiInfo
			   {
				   Title = "Project API",
				   Version = "v1"
			   });

			options.OperationFilter<AcceptLanguageHeaderFilter>();

			options.AddSecurityDefinition(
				bearerScheme,
				new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,

					Scheme = "bearer",

					BearerFormat = "JWT",

					Description = "Enter the JWT access token. " +
								  "You do not need to write the word 'Bearer'."
				});

			options.AddSecurityRequirement(
				new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference =
								new OpenApiReference
								{
									Type =
										ReferenceType.SecurityScheme,

									Id = bearerScheme
								}
						},
						Array.Empty<string>()
					}
				});
		});
	}


	private static void AddServices(this IServiceCollection services)
	{
		services.AddScoped<IRequestContext, HttpRequestContext>();
		services.AddScoped<ICurrentUser, HttpCurrentUser>();
		services.AddScoped<HttpCookieService>();
	}


	private static void AddHttpCookieSettings(this IServiceCollection services,
											  IConfiguration configuration)
	{
		services.AddOptions<HttpCookieSettings>()
				.Bind(configuration.GetSection(HttpCookieSettings.SectionName))
				.Validate(settings => !string.IsNullOrWhiteSpace(settings.Name),
									  "HttpCookieSettings:Name cannot be empty.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.Path),
									  "HttpCookieSettings:Path cannot be empty.")
				
				.ValidateOnStart();
	}

}
