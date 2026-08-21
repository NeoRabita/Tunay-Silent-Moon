using SilentMoonApp.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Configurations;
using SilentMoonApp.Domain.Entities;


namespace SilentMoonApp.Infrastructure.Persistence.Contexts;

public sealed class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options) { }

	public DbSet<User> Users => Set<User>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Topic> Topics => Set<Topic>();
	public DbSet<UserRole> UserRoles => Set<UserRole>();
	public DbSet<UserTopic> UserTopics => Set<UserTopic>();
	public DbSet<ImageFile> ImageFiles => Set<ImageFile>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<UserExternalProvider> UserExternalProviders => Set<UserExternalProvider>();


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ConfigureRelations();
		modelBuilder.ConfigureIndexes();
		modelBuilder.ConfigureTableNames();
	}
}
