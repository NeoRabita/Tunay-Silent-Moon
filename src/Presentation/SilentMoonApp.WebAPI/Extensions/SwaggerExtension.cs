using System.Text;

namespace SilentMoonApp.WebAPI.Extensions;

public static class SwaggerExtension
{
	public static WebApplication UseProtectedSwagger(this WebApplication app)
	{
		bool swaggerEnabled = app.Environment.IsDevelopment()
							  || app.Configuration.GetValue<bool>("Swagger:Enabled");

		if (!swaggerEnabled)
			return app;


		if (!app.Environment.IsDevelopment())
		{
			app.Use(async (context, next) =>
			{
				if (!context.Request.Path.StartsWithSegments("/swagger"))
				{
					await next();
					return;
				}

				string? authorizationHeader = context.Request.Headers.Authorization;

				if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
					authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
				{
					string encodedCredentials = authorizationHeader["Basic ".Length..].Trim();

					string credentials = Encoding.UTF8.GetString(
						Convert.FromBase64String(encodedCredentials));

					string[] parts = credentials.Split(':', 2);

					string username = app.Configuration["Swagger:Username"] ?? string.Empty;
					string password = app.Configuration["Swagger:Password"] ?? string.Empty;

					if (parts.Length == 2 &&
						parts[0] == username &&
						parts[1] == password)
					{
						await next();
						return;
					}
				}

				context.Response.Headers.WWWAuthenticate = "Basic realm=\"Swagger\"";
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			});
		}


		app.UseSwagger();

		app.UseSwaggerUI(options =>
		{
			options.DisplayRequestDuration();
		});

		return app;
	}
}
