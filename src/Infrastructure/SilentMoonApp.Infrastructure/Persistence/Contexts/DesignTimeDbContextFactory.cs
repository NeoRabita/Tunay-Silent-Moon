using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SilentMoonApp.Infrastructure.Persistence.Contexts;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var basePath = Directory.GetCurrentDirectory();

		IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile(
				path: "appsettings.json",
				optional: false,
				reloadOnChange: false)
			.AddJsonFile(
				path: "appsettings.Development.json",
				optional: true,
				reloadOnChange: false)
			.AddEnvironmentVariables()
			.Build();

		var connectionString =
			configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException(
				"Connection String 'DefaultConnection' was not Found.");

		var optionsBuilder =
			new DbContextOptionsBuilder<AppDbContext>();

		optionsBuilder.UseSqlServer(connectionString);

		return new AppDbContext(optionsBuilder.Options);
	}
}
