using Minio;
using System.Text;
using System.Net.Mail;
using StackExchange.Redis;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Application.Settings;
using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Infrastructure.Hashing;
using SilentMoonApp.Infrastructure.Caching;
using SilentMoonApp.Infrastructure.Logging;
using SilentMoonApp.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using SilentMoonApp.Application.Abstractions.Logging;
using SilentMoonApp.Infrastructure.Storage.Providers;
using SilentMoonApp.Application.Abstractions.Caching;
using SilentMoonApp.Application.Abstractions.Hashing;
using SilentMoonApp.Infrastructure.Communication.Email;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Infrastructure.Authentication.Providers;
using SilentMoonApp.Infrastructure.Persistence.Repositories;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Application.Abstractions.Communication.Email;
using SilentMoonApp.Infrastructure.Persistence.Repositories.Read;
using SilentMoonApp.Infrastructure.Persistence.Repositories.Write;


namespace SilentMoonApp.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
															IConfiguration configuration)
	{
		AddDatabase(services, configuration);
		AddSettings(services, configuration);
		AddRepositories(services);
		AddStorage(services, configuration);
		AddExternalServices(services);
		AddSpecialDependencies(services);

		return services;
	}


	private static void AddDatabase(IServiceCollection services,
									IConfiguration configuration)
	{
		string connectionString = configuration.GetConnectionString("DefaultConnection")
							   ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection was not found.");

		services.AddDbContext<AppDbContext>(opt =>
		{
			opt.UseSqlServer(connectionString);
		});
	}



	public static void AddSettings(IServiceCollection services,
								   IConfiguration configuration)
	{
		AddJwtSettings(services, configuration);
		AddAuthSettings(services, configuration);
		AddMailSettings(services, configuration);
		AddOtpSettings(services, configuration);
		AddRedisSettings(services, configuration);
		AddRabbitMqSettings(services, configuration);
		AddStorageSettings(services, configuration);
	}


	private static void AddJwtSettings(IServiceCollection services,
									   IConfiguration configuration)
	{
		services.AddOptions<JwtSettings>()

				.Bind(configuration.GetSection(JwtSettings.SectionName))

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.SecretKey) &&
									   Encoding.UTF8.GetByteCount(settings.SecretKey) >= 32,
									  "JwtSettings: SecretKey must be at least 32 bytes.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.RefreshTokenHmacKey) &&
									   Encoding.UTF8.GetByteCount(settings.RefreshTokenHmacKey) >= 32,
									  "JwtSettings: RefreshTokenHmacKeymust be at least 32 bytes.")

				.Validate(settings => settings.AccessTokenExpirationMinutes > 0,
									  "AccessTokenExpirationMinutes cannot be empty.")

				.Validate(settings => settings.RefreshTokenExpirationDays > 0,
									  "RefreshTokenExpirationDays cannot be empty.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.RefreshTokenCookieName),
									  "RefreshTokenCookieName cannot be empty.")

				.ValidateOnStart();
	}


	private static void AddAuthSettings(IServiceCollection services,
										IConfiguration configuration)
	{
		services.AddOptions<GoogleAuthSettings>()

				.Bind(configuration.GetSection(GoogleAuthSettings.SectionName))

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.ClientId),
									  "GoogleAuthSettings:ClientId cannot be empty.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.ClientSecret),
									  "GoogleAuthSettings:ClientSecret cannot be empty.")

				.ValidateOnStart();



		services.AddOptions<FacebookAuthSettings>()

				.Bind(configuration.GetSection(FacebookAuthSettings.SectionName))

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.AppId),
									  "FacebookAuthSettings:AppId cannot be empty.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.AppSecret),
									  "FacebookAuthSettings:AppSecret cannot be empty.")

				.ValidateOnStart();
	}




	private static void AddMailSettings(IServiceCollection services,
										IConfiguration configuration)
	{
		services.AddOptions<MailSettings>()

				.Bind(configuration.GetSection(MailSettings.SectionName), binderOption =>
				{
					binderOption.ErrorOnUnknownConfiguration = true;
				})

				.Validate(settings => Uri.CheckHostName(settings.Host.Trim()) != UriHostNameType.Unknown,
									 "Mail:Host must be a valid SMTP host name or IP address.")

				.Validate(settings => !settings.Host.Contains("://", StringComparison.Ordinal),
									  "Mail:Host must not contain smtp://, http://, or https://.")

				.Validate(settings => MailAddress.TryCreate(settings.FromAddress, out _),
									 "Mail:FromAddress must be a valid email address.")

				.ValidateOnStart();
	}


	private static void AddOtpSettings(IServiceCollection services,
									   IConfiguration configuration)
	{
		services.AddOptions<OtpSettings>()

				.Bind(configuration.GetRequiredSection(OtpSettings.SectionName))

				.Validate(settings => settings.Length is >= 4 and <= 10,
									  "OtpSettings:Length must be between 4 and 10.")

				.Validate(settings => settings.ExpirationMinutes > 0,
									  "OtpSettings:ExpirationMinutes must be greater than 0.")

				.Validate(settings => settings.MaxFailedAttempts > 0,
									  "OtpSettings:MaxFailedAttempts must be greater than 0.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.OtpHmacKey) &&
									  Encoding.UTF8.GetByteCount(settings.OtpHmacKey) >= 32,
									  "OtpSettings:OtpHmacKey must be at least 32 bytes	.")
				.ValidateOnStart();
	}


	private static void AddStorageSettings(IServiceCollection services,
										   IConfiguration configuration)
	{
		services.AddOptions<StorageSettings>()

				.Bind(configuration.GetRequiredSection(StorageSettings.SectionName))

				.Validate(settings => Enum.IsDefined(settings.DefaultProvider),
										  "StorageSettings:DefaultProvider must be a valid value.")
				.ValidateOnStart();



		if (configuration.GetValue<bool>("StorageSettings:Local:Enabled"))
		{
			services.AddOptions<LocalStorageSettings>()

					.Bind(configuration.GetRequiredSection(LocalStorageSettings.SectionName))

					.ValidateOnStart();


			services.AddScoped<StorageProvider, LocalStorageProvider>();
		}



		if (configuration.GetValue<bool>("StorageSettings:Minio:Enabled"))
		{
			services.AddOptions<MinioStorageSettings>()

					.Bind(configuration.GetRequiredSection(MinioStorageSettings.SectionName))

					.ValidateOnStart();


			services.AddScoped<StorageProvider, MinioStorageProvider>();
		}
	}


	private static void AddRabbitMqSettings(IServiceCollection services,
											IConfiguration configuration)
	{
		services.AddOptions<RabbitMqSettings>()

				.Bind(configuration.GetRequiredSection(RabbitMqSettings.SectionName))

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.Host),
									  "RabbitMqSettings:HostName cannot be empty.")

				.Validate(settings => settings.Port > 0,
									  "RabbitMqSettings:Port must be greater than 0.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.UserName),
									  "RabbitMqSettings:UserName cannot be empty	.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.Password),
									  "RabbitMqSettings:Password cannot be empty.")

				.Validate(settings => !string.IsNullOrWhiteSpace(settings.QueueName),
									  "RabbitMqSettings:QueueName cannot be empty.")

				.ValidateOnStart();
	}


	private static void AddRedisSettings(IServiceCollection services,
										 IConfiguration configuration)
	{
		string redisConnectionString = configuration.GetConnectionString("Redis")

									?? throw new InvalidOperationException("ConnectionStrings:Redis not found.");


		ConfigurationOptions redisOptions = ConfigurationOptions.Parse(redisConnectionString);

		redisOptions.AbortOnConnectFail = false;
		redisOptions.ConnectTimeout = 5000;

		services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisOptions));
	}



	public static void AddStorage(IServiceCollection services,
												IConfiguration configuration)
	{
		StorageSettings storageSettings = configuration.GetRequiredSection(StorageSettings.SectionName)
													   .Get<StorageSettings>()

									   ?? throw new InvalidOperationException($"{StorageSettings.SectionName} configuration not found.");


		MinioStorageSettings? minioSettings = configuration.GetSection(MinioStorageSettings.SectionName)
														   .Get<MinioStorageSettings>();


		LocalStorageSettings? localSettings = configuration.GetSection(LocalStorageSettings.SectionName)
														   .Get<LocalStorageSettings>();

		if (minioSettings?.Enabled is true)

			services.AddMinio(configureClient => configureClient.WithEndpoint(minioSettings.Endpoint)
																.WithCredentials(minioSettings.AccessKey,
																				 minioSettings.SecretKey)
																.WithSSL(minioSettings.UseSSL)
																.Build());


		bool defaultProviderEnabled = storageSettings.DefaultProvider switch
		{
			EStorageProvider.Minio => minioSettings?.Enabled == true,
			EStorageProvider.Local => localSettings?.Enabled == true,

			_ => false
		};


		if (!defaultProviderEnabled)
			throw new InvalidOperationException($"Default storage provider is not enabled: " +
												$"{storageSettings.DefaultProvider}");
	}


	private static void AddRepositories(IServiceCollection services)
	{
		AddBasedScoped(services, "Repository");

		services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
		services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

		services.AddScoped<IUnitOfWork, UnitOfWork>();
	}


	private static void AddExternalServices(IServiceCollection services)
		=> AddBasedScoped(services, "Service");


	private static void AddSpecialDependencies(IServiceCollection services)
	{
		services.AddSingleton(TimeProvider.System);
		services.AddSingleton<ILogMasker, LogMasker>();

		services.AddSingleton<IEmailService, RabbitMqEmailPublisher>();
		services.AddKeyedSingleton<IEmailService, SmtpEmailService>("smtp");
		services.AddHostedService<RabbitMqEmailConsumerWorker>();

		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<IOtpHasher, OtpHasher>();

		services.AddScoped<ICacheService, RedisCacheService>();
		services.AddScoped<IOtpCacheService, RedisOtpCacheService>();

		services.AddSingleton<IExternalAuthProvider, GoogleAuthProvider>();
		services.AddSingleton<IExternalAuthProvider, FacebookAuthProvider>();
	}



	// Helpers

	private static void AddBasedScoped(IServiceCollection services,
									   string scopedItem)
	{
		services.Scan(scan => scan.FromAssemblyOf<AppDbContext>()
								  .AddClasses(classes => classes.Where(type => !type.IsGenericTypeDefinition
																			&& type.Name.EndsWith(scopedItem,
																								  StringComparison.Ordinal)))
				.AsMatchingInterface()
				.WithScopedLifetime());
	}

}
