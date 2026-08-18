using FluentValidation;
using System.Reflection;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.Application.Behaviours;
using SilentMoonApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Abstractions.Storage;


namespace SilentMoonApp.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationLayer(this IServiceCollection service)
	{
		Assembly applicationAssembly = typeof(ValidationBehaviour<,>).Assembly;

		AddHandlers(service, applicationAssembly);
		AddValidators(service, applicationAssembly);
		AddApplicationServices(service);

		return service;
	}

	private static void AddApplicationServices(this IServiceCollection service)
	{
		service.AddScoped<IUserAvatarService, UserAvatarService>();
		service.AddScoped<IAuthTokenService, AuthTokenService>();
		service.AddScoped<IAuthOtpService, AuthOtpService>();
		service.AddScoped<IExternalAuthUserService, ExternalAuthUserService>();
	}



	private static void AddHandlers(this IServiceCollection service, Assembly applicationAssembly)
	{
		service.Scan(scan => scan
			   .FromAssemblies(applicationAssembly)
			   .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
			   .AsImplementedInterfaces()
			   .WithScopedLifetime());


		service.AddScoped<IDispatcher, Dispatcher>();
	}


	private static void AddValidators(this IServiceCollection service, Assembly applicationAssembly)
	{
		service.Scan(scan => scan
			   .FromAssemblies(applicationAssembly)
			   .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
			   .AsImplementedInterfaces()
			   .WithScopedLifetime());


		service.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
		service.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
		service.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
	}

}
