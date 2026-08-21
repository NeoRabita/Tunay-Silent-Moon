using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SilentMoonApp.Infrastructure.Persistence.Contexts;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var basePath = Directory.GetCurrentDirectory();

		string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
							  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
							  ?? "Development";

		IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile(path: "appsettings.json",
						 optional: false,
						 reloadOnChange: false)
			.AddJsonFile(path: $"appsettings.{environmentName}.json",
						 optional: true,
						 reloadOnChange: false)
			.AddEnvironmentVariables()
			.Build();

		string connectionString = configuration.GetConnectionString("OracleConnection")
							   //?? configuration.GetConnectionString("DefaultConnection")
							   ?? throw new InvalidOperationException("ConnectionStrings were not found.");

		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

		//optionsBuilder.UseSqlServer(connectionString);
		optionsBuilder.UseOracle(connectionString);

		return new AppDbContext(optionsBuilder.Options);
	}
}
