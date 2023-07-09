using AnimeVnInfoBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnimeVnInfoBackend.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<AdminNavigation> AdminNavigations { get; set; }
        public DbSet<Anime> Animes { get; set; }
        public DbSet<AnimeCharacter> AnimeCharacters { get; set; }
        public DbSet<AnimeGenre> AnimeGenres { get; set; }
        public DbSet<AnimeImage> AnimeImages { get; set; }
        public DbSet<AnimeListStatus> AnimeListStatuss { get; set; }
        public DbSet<AnimeProducer> AnimeProducers { get; set; }
        public DbSet<AnimeRelation> AnimeRelations { get; set; }
        public DbSet<AnimeSource> AnimeSources { get; set; }
        public DbSet<AnimeStaff> AnimeStaffs { get; set; }
        public DbSet<AnimeStatus> AnimeStatuss { get; set; }
        public DbSet<AnimeStudio> AnimeStudios { get; set; }
        public DbSet<AnimeType> AnimeTypes { get; set; }
        public DbSet<AnimeVideo> AnimeVideos { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterImage> CharacterImages { get; set; }
        public DbSet<CharacterStaff> CharacterStaffs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Logger> Loggers { get; set; }
        public DbSet<MainNavigation> MainNavigations { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<ProducerImage> ProducerImages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RelationType> RelationTypes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewScore> ReviewScores { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffImage> StaffImages { get; set; }
        public DbSet<StaffJob> StaffJobs { get; set; }
        public DbSet<Studio> Studios { get; set; }
        public DbSet<StudioImage> StudioImages { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set foreign keys
            modelBuilder.Entity<AnimeImage>()
                .HasOne(s => s.Anime)
                .WithMany(to => to.AnimeImage)
                .HasForeignKey(to => to.AnimeId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeCharacter>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.AnimeCharacter)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeCharacter>()
               .HasOne(s => s.Character)
               .WithMany(to => to.AnimeCharacter)
               .HasForeignKey(to => to.CharacterId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeGenre>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.AnimeGenre)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeGenre>()
               .HasOne(s => s.Genre)
               .WithMany(to => to.AnimeGenre)
               .HasForeignKey(to => to.GenreId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeProducer>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.AnimeProducer)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeProducer>()
               .HasOne(s => s.Producer)
               .WithMany(to => to.AnimeProducer)
               .HasForeignKey(to => to.ProducerId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeStudio>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.AnimeStudio)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeStudio>()
               .HasOne(s => s.Studio)
               .WithMany(to => to.AnimeStudio)
               .HasForeignKey(to => to.StudioId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Anime>()
               .HasOne(s => s.AnimeSource)
               .WithMany(to => to.Anime)
               .HasForeignKey(to => to.AnimeSourceId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Anime>()
               .HasOne(s => s.AnimeType)
               .WithMany(to => to.Anime)
               .HasForeignKey(to => to.AnimeTypeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Anime>()
               .HasOne(s => s.AnimeStatus)
               .WithMany(to => to.Anime)
               .HasForeignKey(to => to.AnimeStatusId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Anime>()
               .HasOne(s => s.Rating)
               .WithMany(to => to.Anime)
               .HasForeignKey(to => to.RatingId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Anime>()
               .HasOne(s => s.Season)
               .WithMany(to => to.Anime)
               .HasForeignKey(to => to.SeasonId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<CharacterImage>()
               .HasOne(s => s.Character)
               .WithMany(to => to.CharacterImage)
               .HasForeignKey(to => to.CharacterId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<UserInfo>()
               .HasOne(s => s.User)
               .WithOne(to => to.UserInfo)
               .HasForeignKey<UserInfo>(to => to.UserId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<StudioImage>()
               .HasOne(s => s.Studio)
               .WithMany(to => to.StudioImage)
               .HasForeignKey(to => to.StudioId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ProducerImage>()
               .HasOne(s => s.Producer)
               .WithMany(to => to.ProducerImage)
               .HasForeignKey(to => to.ProducerId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Review>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.Review)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Review>()
               .HasOne(s => s.User)
               .WithMany(to => to.Review)
               .HasForeignKey(to => to.UserId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Review>()
               .HasOne(s => s.ReviewScore)
               .WithMany(to => to.Review)
               .HasForeignKey(to => to.ReviewScoreId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Staff>()
               .HasOne(s => s.Language)
               .WithMany(to => to.Staff)
               .HasForeignKey(to => to.LanguageId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<StaffImage>()
               .HasOne(s => s.Staff)
               .WithMany(to => to.StaffImage)
               .HasForeignKey(to => to.StaffId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<CharacterStaff>()
               .HasOne(s => s.Staff)
               .WithMany(to => to.CharacterStaff)
               .HasForeignKey(to => to.StaffId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<CharacterStaff>()
               .HasOne(s => s.Character)
               .WithMany(to => to.CharacterStaff)
               .HasForeignKey(to => to.CharacterId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AdminNavigation>()
               .HasOne(s => s.Role)
               .WithMany(to => to.AdminNavigation)
               .HasForeignKey(to => to.RoleId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeRelation>()
               .HasOne(s => s.ChildAnime)
               .WithMany(to => to.ChildAnimeRelation)
               .HasForeignKey(to => to.ChildId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeRelation>()
               .HasOne(s => s.ParentAnime)
               .WithMany(to => to.ParentAnimeRelation)
               .HasForeignKey(to => to.ParentId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<StaffJob>()
               .HasOne(s => s.Staff)
               .WithMany(to => to.StaffJob)
               .HasForeignKey(to => to.StaffId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<StaffJob>()
               .HasOne(s => s.Job)
               .WithMany(to => to.StaffJob)
               .HasForeignKey(to => to.JobId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeStaff>()
               .HasOne(s => s.Staff)
               .WithMany(to => to.AnimeStaff)
               .HasForeignKey(to => to.StaffId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeStaff>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.AnimeStaff)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeRelation>()
               .HasOne(s => s.RelationType)
               .WithMany(to => to.AnimeRelation)
               .HasForeignKey(to => to.RelationTypeId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Anime>()
               .HasOne(s => s.Language)
               .WithMany(to => to.Anime)
               .HasForeignKey(to => to.LanguageId)
               .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<AnimeVideo>()
               .HasOne(s => s.Anime)
               .WithMany(to => to.AnimeVideo)
               .HasForeignKey(to => to.AnimeId)
               .OnDelete(DeleteBehavior.SetNull);


            // Set indexes
            modelBuilder.Entity<User>()
                .HasIndex(s => s.UserName);

            SeedRoles(modelBuilder);
            SeedUsers(modelBuilder);
            SeedUsersRoles(modelBuilder);
            SeedUserInfos(modelBuilder);
            SeedAdminNavLists(modelBuilder);
        }

        //Data seed
        private void SeedRoles(ModelBuilder modelBuilder)
        {
            Role newAdminRole = new()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Id = 1,
                Name = "Admin"
            };
            Role newModeratorRole = new()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Id = 2,
                Name = "Moderator"
            };
            Role newUserRole = new()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Id = 3,
                Name = "User"
            };
            modelBuilder.Entity<Role>().HasData(newAdminRole);
            modelBuilder.Entity<Role>().HasData(newModeratorRole);
            modelBuilder.Entity<Role>().HasData(newUserRole);
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            User newUser = new()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "animuvn.info@gmail.com",
                NormalizedEmail = "ANIMUVN.INFO@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                TwoFactorEnabled = true
            };
            PasswordHasher<User> password = new();
            newUser.PasswordHash = password.HashPassword(newUser, "adminadmin");
            modelBuilder.Entity<User>().HasData(newUser);

            User newUser2 = new User
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Id = 2,
                UserName = "admin2",
                NormalizedUserName = "ADMIN2",
                Email = "nmc0401@gmail.com",
                NormalizedEmail = "NMC0401@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                TwoFactorEnabled = true
            };
            PasswordHasher<User> password2 = new PasswordHasher<User>();
            newUser2.PasswordHash = password2.HashPassword(newUser, "123456@A");
            modelBuilder.Entity<User>().HasData(newUser2);
        }

        private void SeedUsersRoles(ModelBuilder modelBuilder)
        {
            IdentityUserRole<int> newUserRoles = new()
            {
                UserId = 1,
                RoleId = 1
            };
            IdentityUserRole<int> newUserRoles1 = new()
            {
                UserId = 1,
                RoleId = 2
            };
            IdentityUserRole<int> newUserRoles2 = new()
            {
                UserId = 1,
                RoleId = 3
            };
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(newUserRoles);
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(newUserRoles1);
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(newUserRoles2);

            IdentityUserRole<int> newUser2Roles1 = new IdentityUserRole<int>
            {
                UserId = 2,
                RoleId = 2
            };
            IdentityUserRole<int> newUser2Roles2 = new IdentityUserRole<int>
            {
                UserId = 2,
                RoleId = 3
            };
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(newUser2Roles1);
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(newUser2Roles2);
        }

        private void SeedUserInfos(ModelBuilder modelBuilder)
        {
            UserInfo newUserInfo = new()
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = 1
            };
            modelBuilder.Entity<UserInfo>().HasData(newUserInfo);

            UserInfo newUserInfo2 = new()
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = 2
            };
            modelBuilder.Entity<UserInfo>().HasData(newUserInfo2);
        }

        private void SeedAdminNavLists(ModelBuilder modelBuilder)
        {
            AdminNavigation newAdminNavigation = new()
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Title = "Quản lý danh mục",
                Order = 1,
                Link = "/navigation-management",
                RoleId = 1
            };
            AdminNavigation newAdminNavigation2 = new()
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Title = "Danh mục admin",
                Order = 1,
                Link = "/navigation-management/admin",
                ParentId = 1,
                RoleId = 1
            };
            AdminNavigation newAdminNavigation3 = new()
            {
                Id = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Title = "Danh mục chính",
                Order = 2,
                Link = "/navigation-management/main",
                ParentId = 1,
                RoleId = 1
            };
            modelBuilder.Entity<AdminNavigation>().HasData(newAdminNavigation);
            modelBuilder.Entity<AdminNavigation>().HasData(newAdminNavigation2);
            modelBuilder.Entity<AdminNavigation>().HasData(newAdminNavigation3);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public ApplicationDbContext()
        {

        }
    }
}
