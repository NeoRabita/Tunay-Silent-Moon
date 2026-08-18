namespace SilentMoonApp.WebAPI.Extensions;

public static class LocalizationExtensions
{
	public static void UserLocalization(this IApplicationBuilder app)
	{
		var supportedCultures = new[] { "en-US", "az-AZ" ,"ru-RU",
										"en", "az", "ru"};

		var localizationOptions = new RequestLocalizationOptions()
			.SetDefaultCulture(supportedCultures[0])
			.AddSupportedCultures(supportedCultures)
			.AddSupportedUICultures(supportedCultures);
		
		localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

		app.UseRequestLocalization(localizationOptions);
	}
}
