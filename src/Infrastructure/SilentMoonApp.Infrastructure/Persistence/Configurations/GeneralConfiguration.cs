using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Domain;
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

		builder.Entity<User>()
			   .HasMany(user => user.Reminders)
			   .WithOne(reminder => reminder.User)
			   .HasForeignKey(reminder => reminder.UserId)
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



		// Category Relations

		builder.Entity<Category>()
			   .HasOne(category => category.IconFile)
			   .WithOne()
			   .HasForeignKey<Category>(category => category.IconFileId)
			   .IsRequired(false)
			   .OnDelete(DeleteBehavior.NoAction);



		// CategoryType Relations

		builder.Entity<CategoryType>()
			   .HasMany(categoryType => categoryType.Categories)
			   .WithOne(category => category.CategoryType)
			   .HasForeignKey(category => category.CategoryTypeId)
			   .OnDelete(DeleteBehavior.NoAction);



		// Course Relations

		builder.Entity<Course>()
			   .HasOne(course => course.Category)
			   .WithMany(category => category.Courses)
			   .HasForeignKey(course => course.CategoryId)
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Course>()
			   .HasOne(course => course.CoverImageFile)
			   .WithOne()
			   .HasForeignKey<Course>(course => course.CoverImageFileId)
			   .IsRequired()
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Course>()
			   .HasMany(course => course.Tracks)
			   .WithOne(track => track.Course)
			   .HasForeignKey(track => track.CourseId)
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Course>()
			   .HasMany(course => course.CourseFavorites)
			   .WithOne(favorite => favorite.Course)
			   .HasForeignKey(favorite => favorite.CourseId)
			   .OnDelete(DeleteBehavior.NoAction);



		// Track Relations

		builder.Entity<Track>()
			   .HasOne(track => track.Narrator)
			   .WithMany(narrator => narrator.Tracks)
			   .HasForeignKey(track => track.NarratorId)
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Track>()
			   .HasOne(track => track.AudioFile)
			   .WithOne()
			   .HasForeignKey<Track>(track => track.AudioFileId)
			   .IsRequired()
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Track>()
			   .HasOne(track => track.CoverImageFile)
			   .WithOne()
			   .HasForeignKey<Track>(track => track.CoverImageFileId)
			   .IsRequired(false)
			   .OnDelete(DeleteBehavior.NoAction);

		builder.Entity<Track>()
			   .HasMany(track => track.TrackProgresses)
			   .WithOne(progress => progress.Track)
			   .HasForeignKey(progress => progress.TrackId)
			   .OnDelete(DeleteBehavior.NoAction);



		// CourseFavorite Relations

		builder.Entity<CourseFavorite>()
			   .HasOne(favorite => favorite.User)
			   .WithMany()
			   .HasForeignKey(favorite => favorite.UserId)
			   .OnDelete(DeleteBehavior.NoAction);



		// TrackProgress Relations

		builder.Entity<TrackProgress>()
			   .HasOne(progress => progress.User)
			   .WithMany()
			   .HasForeignKey(progress => progress.UserId)
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
				.HasMaxLength(256); // NEW: nvarchar(max) index ���n problem yaradir

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
			   .Property(file => file.ContainerName)
			   .HasMaxLength(128);

		builder.Entity<ImageFile>()
			   .Property(file => file.StoredFileName)
			   .HasMaxLength(1024);

		builder.Entity<ImageFile>()
			   .Property(file => file.UploadedFileName)
			   .HasMaxLength(255);

		builder.Entity<ImageFile>()
			   .Property(file => file.Extension)
			   .HasMaxLength(16);

		builder.Entity<ImageFile>()
			   .Property(file => file.ContentType)
			   .HasMaxLength(128);

		builder.Entity<ImageFile>()
			   .HasIndex(file => new
			   {
				   file.ContainerName,
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



		// Reminder Indexes

		builder.Entity<Reminder>()
			   .Property(reminder => reminder.Label)
			   .HasMaxLength(100);



		// Category Indexes

		builder.Entity<Category>()
			   .Property(category => category.Slug)
			   .HasMaxLength(128);

		builder.Entity<Category>()
			   .Property(category => category.Title)
			   .HasMaxLength(128);

		builder.Entity<Category>()
			   .HasIndex(category => new
			   {
				   category.CategoryTypeId,
				   category.Slug
			   })
			   .IsUnique();

		builder.Entity<Category>()
			   .HasIndex(category => category.IconFileId)
			   .IsUnique();



		// CategoryType Indexes

		builder.Entity<CategoryType>()
			   .Property(categoryType => categoryType.Slug)
			   .HasMaxLength(64);

		builder.Entity<CategoryType>()
			   .Property(categoryType => categoryType.Title)
			   .HasMaxLength(128);

		builder.Entity<CategoryType>()
			   .HasIndex(categoryType => categoryType.Slug)
			   .IsUnique();



		// Course Indexes

		builder.Entity<Course>()
			   .Property(course => course.Title)
			   .HasMaxLength(128);

		builder.Entity<Course>()
			   .Property(course => course.SubTitle)
			   .HasMaxLength(256);

		builder.Entity<Course>()
			   .Property(course => course.Description)
			   .HasMaxLength(1000);

		builder.Entity<Course>()
			   .HasIndex(course => course.CategoryId);

		builder.Entity<Course>()
			   .HasIndex(course => course.CoverImageFileId)
			   .IsUnique();

		builder.Entity<Course>()
			   .HasIndex(course => course.CreatedAt);

		builder.Entity<Course>()
			   .HasIndex(course => course.IsFeatured);



		// Narrator Indexes

		builder.Entity<Narrator>()
			   .Property(narrator => narrator.Name)
			   .HasMaxLength(128);

		builder.Entity<Narrator>()
			   .Property(narrator => narrator.Slug)
			   .HasMaxLength(64);

		builder.Entity<Narrator>()
			   .HasIndex(narrator => narrator.Slug)
			   .IsUnique();



		// Track Indexes

		builder.Entity<Track>()
			   .Property(track => track.Title)
			   .HasMaxLength(128);

		builder.Entity<Track>()
			   .HasIndex(track => track.CourseId);

		builder.Entity<Track>()
			   .HasIndex(track => track.NarratorId);

		builder.Entity<Track>()
			   .HasIndex(track => track.AudioFileId)
			   .IsUnique();

		builder.Entity<Track>()
			   .HasIndex(track => track.CoverImageFileId)
			   .IsUnique();

		builder.Entity<Track>()
			   .HasIndex(track => new
			   {
				   track.CourseId,
				   track.Order
			   })
			   .IsUnique();



		// CourseFavorite Indexes

		builder.Entity<CourseFavorite>()
			   .HasIndex(favorite => new
			   {
				   favorite.UserId,
				   favorite.CourseId
			   })
			   .IsUnique();



		// TrackProgress Indexes

		builder.Entity<TrackProgress>()
			   .HasIndex(progress => new
			   {
				   progress.UserId,
				   progress.TrackId
			   })
			   .IsUnique();



		// AudioFile Indexes

		builder.Entity<AudioFile>()
			   .Property(file => file.ContainerName)
			   .HasMaxLength(128);

		builder.Entity<AudioFile>()
			   .Property(file => file.StoredFileName)
			   .HasMaxLength(1024);

		builder.Entity<AudioFile>()
			   .Property(file => file.UploadedFileName)
			   .HasMaxLength(255);

		builder.Entity<AudioFile>()
			   .Property(file => file.Extension)
			   .HasMaxLength(16);

		builder.Entity<AudioFile>()
			   .Property(file => file.ContentType)
			   .HasMaxLength(128);

		builder.Entity<AudioFile>()
			   .HasIndex(file => new
			   {
				   file.ContainerName,
				   file.StoredFileName
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

		builder.Entity<ImageFile>()
			   .ToTable("ImageFiles");

		builder.Entity<RefreshToken>()
			   .ToTable("RefreshTokens");

		builder.Entity<Topic>()
			   .ToTable("Topics");

		builder.Entity<UserTopic>()
			   .ToTable("UserTopics");

		builder.Entity<Category>()
			   .ToTable("Categories");

		builder.Entity<CategoryType>()
			   .ToTable("CategoryTypes");

		builder.Entity<AudioFile>()
			   .ToTable("AudioFiles");

		builder.Entity<Course>()
			   .ToTable("Courses");

		builder.Entity<Track>()
			   .ToTable("Tracks");

		builder.Entity<Narrator>()
			   .ToTable("Narrators");

		builder.Entity<CourseFavorite>()
			   .ToTable("CourseFavorites");

		builder.Entity<TrackProgress>()
			   .ToTable("TrackProgresses");
	}

}
