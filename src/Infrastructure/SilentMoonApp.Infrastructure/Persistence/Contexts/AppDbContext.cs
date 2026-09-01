using SilentMoonApp.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Configurations;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Domain;


namespace SilentMoonApp.Infrastructure.Persistence.Contexts;

public sealed class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options) { }

	public DbSet<User> Users => Set<User>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Topic> Topics => Set<Topic>();
	public DbSet<Track> Tracks => Set<Track>();
	public DbSet<Course> Courses => Set<Course>();
	public DbSet<Narrator> Narrators => Set<Narrator>();
	public DbSet<Reminder> Reminders => Set<Reminder>();
	public DbSet<UserRole> UserRoles => Set<UserRole>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<UserTopic> UserTopics => Set<UserTopic>();
	public DbSet<AudioFile> AudioFiles => Set<AudioFile>();
	public DbSet<ImageFile> ImageFiles => Set<ImageFile>();
	public DbSet<CategoryType> CategoryTypes => Set<CategoryType>();
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
