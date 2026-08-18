using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SilentMoonApp.Application.Abstractions.Hashing;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Seed;

public static class DefaultSchemaSeeder
{
	private const string AdminEmailKey = "DefaultSchema:Admin:Email";
	private const string AdminPasswordKey = "DefaultSchema:Admin:Password";
	private const string AdminFirstNameKey = "DefaultSchema:Admin:FirstName";
	private const string AdminLastNameKey = "DefaultSchema:Admin:LastName";
	private const string AdminUserNameKey = "DefaultSchema:Admin:UserName";

	private const string DefaultAdminEmail = "admin@project.local";
	private const string DefaultAdminPassword = "Admin123!";
	private const string DefaultAdminFirstName = "System";
	private const string DefaultAdminLastName = "Administrator";
	private const string DefaultAdminUserName = "admin";


	public static async Task SeedDefaultSchemaAsync(this IServiceProvider serviceProvider,
													CancellationToken cancellationToken = default)
	{
		using IServiceScope scope = serviceProvider.CreateScope();


		AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

		IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();


		await dbContext.Database.MigrateAsync(cancellationToken);

		DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

		Dictionary<EUserRole, Role> roles = await SeedRolesAsync(dbContext: dbContext,
																 nowUtc: nowUtc,
																 cancellationToken: cancellationToken);

		User adminUser = await SeedAdminUserAsync(dbContext,
												  configuration,
												  passwordHasher,
												  nowUtc,
												  cancellationToken);

		await SeedAdminRoleAsync(dbContext,
								 adminUser,
								 roles[EUserRole.Admin],
								 nowUtc,
								 cancellationToken);


		await dbContext.SaveChangesAsync(cancellationToken);
	}



	// Helpers

	private static async Task<Dictionary<EUserRole, Role>> SeedRolesAsync(AppDbContext dbContext,
																		  DateTimeOffset nowUtc,
																		  CancellationToken cancellationToken)
	{
		EUserRole[] defaultRoles =
		[
			EUserRole.Admin,
			EUserRole.User,
			EUserRole.Guest
		];


		List<Role> existingRoles = await dbContext.Roles.ToListAsync(cancellationToken);


		foreach (EUserRole userRole in defaultRoles)
		{
			if (existingRoles.Any(role => role.Name == userRole))
				continue;


			Role role = new()
			{
				Name = userRole,
				NormalizedName = userRole.ToString().ToUpperInvariant(),
				Description = $"{userRole} role",
				CreatedAt = nowUtc
			};

			dbContext.Roles.Add(role);

			existingRoles.Add(role);
		}


		return existingRoles.ToDictionary(role => role.Name);
	}


	private static async Task<User> SeedAdminUserAsync(AppDbContext dbContext,
													   IConfiguration configuration,
													   IPasswordHasher passwordHasher,
													   DateTimeOffset nowUtc,
													   CancellationToken cancellationToken)
	{
		string adminEmail = GetConfigurationValue(configuration,
												  AdminEmailKey,
												  DefaultAdminEmail).Trim().ToLowerInvariant();

		User? adminUser = await dbContext.Users.Include(user => user.UserRoles)
											   .ThenInclude(userRole => userRole.Role)
											   .FirstOrDefaultAsync(user => user.Email == adminEmail, cancellationToken);

		if (adminUser is not null)
			return adminUser;


		string adminUserName = GetConfigurationValue(configuration: configuration,
													 key: AdminUserNameKey,
													 fallbackValue: DefaultAdminUserName);

		bool adminUserNameExists = await dbContext.Users.AnyAsync(user => user.UserName == adminUserName,
																  cancellationToken);

		adminUser = new User
		{
			FirstName = GetConfigurationValue(configuration, AdminFirstNameKey, DefaultAdminFirstName),
			LastName = GetConfigurationValue(configuration, AdminLastNameKey, DefaultAdminLastName),
			UserName = adminUserNameExists ? null : adminUserName,

			Email = adminEmail,
			PasswordHash = passwordHasher.Hash(GetConfigurationValue(configuration, AdminPasswordKey, DefaultAdminPassword)),

			UserStatus = EUserStatus.Active,

			IsEmailConfirmed = true,
			AccessFailedCount = 0,

			LockoutEndAt = null,
			ConfirmedAt = nowUtc,

			CreatedAt = nowUtc,
			UpdatedAt = null,

			IsDeleted = false
		};

		dbContext.Users.Add(adminUser);


		return adminUser;
	}


	private static async Task SeedAdminRoleAsync(AppDbContext dbContext,
												 User adminUser,
												 Role adminRole,
												 DateTimeOffset nowUtc,
												 CancellationToken cancellationToken)
	{
		bool adminRoleExists = await dbContext.UserRoles.AnyAsync(predicate: userRole => userRole.UserId == adminUser.Id && userRole.RoleId == adminRole.Id,
																  cancellationToken: cancellationToken);

		if (adminRoleExists)
			return;


		dbContext.UserRoles.Add(new UserRole
		{
			User = adminUser,
			Role = adminRole,
			CreatedAt = nowUtc
		});
	}


	private static string GetConfigurationValue(IConfiguration configuration,
												string key,
												string fallbackValue)
	{
		string? value = configuration[key];

		return string.IsNullOrWhiteSpace(value)
			? fallbackValue
			: value;
	}

}
