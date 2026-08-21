using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Domain.Entities.Files;
using SilentMoonApp.Domain.Entities.Identity;


namespace SilentMoonApp.Infrastructure.Persistence.Configurations;

public static class GeneralConfiguration
{
	public static void ConfigureRelations(this ModelBuilder builder)
	{
		// User Relations

		builder.Entity<User>()
			   .HasMany(user => user.RefreshTokens)
			   .WithOne(refreshToken => refreshToken.User)
			   .HasForeignKey(refreshToken => refreshToken.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.Entity<User>()
			   .HasMany(user => user.UserRoles)
			   .WithOne(userRole => userRole.User)
			   .HasForeignKey(userRole => userRole.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.Entity<User>()
			   .HasOne(user => user.AvatarImageFile)
			   .WithOne()
			   .HasForeignKey<User>(user => user.AvatarImageFileId)
			   .IsRequired(false)
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<User>()
			   .HasMany(user => user.UserExternalProviders)
			   .WithOne(provider => provider.User)
			   .HasForeignKey(provider => provider.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.Entity<User>()
			   .HasMany(user => user.UserTopics)
			   .WithOne(userTopic => userTopic.User)
			   .HasForeignKey(userTopic => userTopic.UserId)
			   .OnDelete(DeleteBehavior.Cascade);



		// Role Relations

		builder.Entity<Role>()
			   .HasMany(role => role.UserRoles)
			   .WithOne(userRole => userRole.Role)
			   .HasForeignKey(userRole => userRole.RoleId)
			   .OnDelete(DeleteBehavior.Cascade);



		// RefreshToken Relations

		builder.Entity<RefreshToken>()
			   .HasOne<RefreshToken>()
			   .WithOne()
			   .HasForeignKey<RefreshToken>(refreshToken => refreshToken.ReplacedTokenId)
			   .OnDelete(DeleteBehavior.NoAction);



		// Topic Relations

		builder.Entity<Topic>()
			   .HasMany(topic => topic.UserTopics)
			   .WithOne(userTopic => userTopic.Topic)
			   .HasForeignKey(userTopic => userTopic.TopicId)
			   .OnDelete(DeleteBehavior.NoAction);
	}


	public static void ConfigureIndexes(this ModelBuilder builder)
	{
		// User Indexes

		builder.Entity<User>()
			   .HasIndex(user => user.Email)
			   .IsUnique();

		builder.Entity<User>()
			   .HasIndex(user => user.UserName)
			   .IsUnique()
			   .HasFilter("[UserName] IS NOT NULL");

		builder.Entity<User>()
			   .HasIndex(user => user.AvatarImageFileId)
			   .IsUnique()
			   .HasFilter("[AvatarImageFileId] IS NOT NULL");



		// UserExternalProvider Indexes
		
		builder.Entity<UserExternalProvider>()
				.Property(provider => provider.ProviderUserId)
				.HasMaxLength(256); // NEW: nvarchar(max) index üçün problem yaradir

		builder.Entity<UserExternalProvider>()
				.HasIndex(provider => new
				{
					provider.Provider,
					provider.ProviderUserId
				})
				.IsUnique();



		// Role Indexes

		builder.Entity<Role>()
			   .HasIndex(role => role.Name)
			   .IsUnique();

		builder.Entity<Role>()
			   .HasIndex(role => role.NormalizedName)
			   .IsUnique();

		builder.Entity<UserRole>()
			   .HasIndex(userRole => new
			   {
				   userRole.UserId,
				   userRole.RoleId
			   })
			   .IsUnique();


		// RefreshToken Indexes

		builder.Entity<RefreshToken>()
			   .HasIndex(refreshToken => refreshToken.TokenHash)
			   .IsUnique();


		// ImageFile Indexes

		builder.Entity<ImageFile>()
			   .HasIndex(file => new
			   {
				   file.StoredFileName
			   })
			   .IsUnique();

		// Topic Indexes

		builder.Entity<Topic>()
			   .HasIndex(topic => topic.Slug)
			   .IsUnique();


		// UserTopic Indexes

		builder.Entity<UserTopic>()
			   .HasIndex(userTopic => new
			   {
			  	   userTopic.UserId,
			  	   userTopic.TopicId
			   })
			   .IsUnique();
	}


	public static void ConfigureTableNames(this ModelBuilder builder)
	{
		// Table Names

		builder.Entity<User>()
			   .ToTable("Users");

		builder.Entity<Role>()
			   .ToTable("Roles");

		builder.Entity<UserRole>()
			   .ToTable("UserRoles");

		builder.Entity<RefreshToken>()
			   .ToTable("RefreshTokens");

		builder.Entity<ImageFile>()
			   .ToTable("ImageFiles");

		builder.Entity<Topic>()
			   .ToTable("Topics");

		builder.Entity<UserTopic>()
			   .ToTable("UserTopics");	
	}

}
