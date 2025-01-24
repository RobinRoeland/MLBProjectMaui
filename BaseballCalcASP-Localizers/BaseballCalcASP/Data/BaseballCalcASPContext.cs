using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseballModelsLib.Models;
using BaseballCalcASP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BaseballCalcASP.Utils;
using CsvHelper.Configuration;
using CsvHelper;

namespace BaseballCalcASP.Data
{
    public class BaseballCalcASPContext : IdentityDbContext
    {
        public BaseballCalcASPContext (DbContextOptions<BaseballCalcASPContext> options)
            : base(options)
        {
        }

        public DbSet<BaseballModelsLib.Models.Team> Teams { get; set; } = default!;

        public DbSet<BaseballModelsLib.Models.Player>? Players { get; set; }

        public DbSet<BaseballModelsLib.Models.Season>? Seasons { get; set; }

        public DbSet<BaseballModelsLib.Models.Game>? Games { get; set; }

        public DbSet<BaseballModelsLib.Models.ScoreStatistic>? Statistics { get; set; }

        // any unique string id
        string ADMIN_ID = Guid.NewGuid().ToString("D");
        string USR1_ID = Guid.NewGuid().ToString("D");
        string USR2_ID = Guid.NewGuid().ToString("D");

        string ADMINROLE_ID = Guid.NewGuid().ToString("D");
        string USERROLE_ID = Guid.NewGuid().ToString("D");

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())//.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("BaseballCalcASPContext");
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //make the primary key of scorestatistic a unique combination of attributes
            modelBuilder.Entity<ScoreStatistic>()
                .HasKey(p => new { p.GameId, p.Inning, p.InningTop, p.PersonMLBId, p.ScoreIdInInning });
            modelBuilder.Entity<Team>()
                .Property(t => t.Id)
                .ValueGeneratedNever(); // Disable auto-increment
            modelBuilder.Entity<Game>()
                .Property(t => t.Id)
                .ValueGeneratedNever(); // Disable auto-increment
            modelBuilder.Entity<Player>()
                .Property(t => t.Id)
                .ValueGeneratedNever(); // Disable auto-increment
            modelBuilder.Entity<Season>()
                .Property(t => t.Id)
                .ValueGeneratedNever(); // Disable auto-increment

            base.OnModelCreating(modelBuilder);

            this.SeedRoles(modelBuilder);
            this.SeedUsers(modelBuilder);
            this.SeedUserRoles(modelBuilder);

            this.SeedTeams(modelBuilder);
            this.SeedPlayers(modelBuilder);
            this.SeedSeasons(modelBuilder);
            this.SeedGames(modelBuilder);
            this.SeedStatistics(modelBuilder);
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = ADMINROLE_ID,
                Name = "admin",
                NormalizedName = "admin"
            },
            new IdentityRole
            {
                Id = USERROLE_ID,
                Name = "user",
                NormalizedName = "user"
            });
        }
        private void SeedUsers(ModelBuilder modelBuilder)
        {
            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = ADMIN_ID,
                UserName = "admin@testemail.com",
                NormalizedUserName = "ADMIN@TESTEMAIL.COM",
                Email = "admin@testemail.com",
                NormalizedEmail = "ADMIN@TESTEMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Start123#"),
                SecurityStamp = Guid.NewGuid().ToString("D"),
                PhoneNumberConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                deleted = false
            },
            new AppUser
            {
                Id = USR1_ID,
                UserName = "user1@testemail.com",
                NormalizedUserName = "USER1@TESTEMAIL.COM",
                Email = "user1@testemail.com",
                NormalizedEmail = "USER1@TESTEMAIL.COM",
                EmailConfirmed = true,//false,
                PasswordHash = hasher.HashPassword(null, "Start123#"),
                SecurityStamp = Guid.NewGuid().ToString("D"), //string.Empty,
                PhoneNumberConfirmed = true,
                FirstName = "User1",
                LastName = "AppUser1",
                deleted = false
            },
            new AppUser
            {
                Id = USR2_ID,
                UserName = "user2@testemail.com",
                NormalizedUserName = "USER2@TESTEMAIL.COM",
                Email = "user2@testemail.com",
                NormalizedEmail = "USER2@TESTEMAIL.COM",
                EmailConfirmed = true,//false,
                PasswordHash = hasher.HashPassword(null, "Start123#"),
                SecurityStamp = Guid.NewGuid().ToString("D"), //string.Empty,
                PhoneNumberConfirmed = true,
                FirstName = "User2",
                LastName = "AppUser2",
                deleted = false
            });
        }
        private void SeedUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = ADMINROLE_ID,
                UserId = ADMIN_ID
            },
            new IdentityUserRole<string>
            {
                RoleId = USERROLE_ID,
                UserId = USR1_ID
            },
            new IdentityUserRole<string>
            {
                RoleId = USERROLE_ID,
                UserId = USR2_ID
            });
        }

        private void SeedTeams(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasData(
                new Team { Id = 108, Name = "Los Angeles Angels", TotalPlayers = 40, Deleted = false },
                new Team { Id = 109, Name = "Arizona Diamondbacks", TotalPlayers = 36, Deleted = false },
                new Team { Id = 110, Name = "Baltimore Orioles", TotalPlayers = 39, Deleted = false },
                new Team { Id = 111, Name = "Boston Red Sox", TotalPlayers = 40, Deleted = false },
                new Team { Id = 112, Name = "Chicago Cubs", TotalPlayers = 40, Deleted = false },
                new Team { Id = 113, Name = "Cincinnati Reds", TotalPlayers = 38, Deleted = false },
                new Team { Id = 114, Name = "Cleveland Guardians", TotalPlayers = 39, Deleted = false },
                new Team { Id = 115, Name = "Colorado Rockies", TotalPlayers = 39, Deleted = false },
                new Team { Id = 116, Name = "Detroit Tigers", TotalPlayers = 40, Deleted = false },
                new Team { Id = 117, Name = "Houston Astros", TotalPlayers = 40, Deleted = false },
                new Team { Id = 118, Name = "Kansas City Royals", TotalPlayers = 38, Deleted = false },
                new Team { Id = 119, Name = "Los Angeles Dodgers", TotalPlayers = 39, Deleted = false },
                new Team { Id = 120, Name = "Washington Nationals", TotalPlayers = 37, Deleted = false },
                new Team { Id = 121, Name = "New York Mets", TotalPlayers = 34, Deleted = false },
                new Team { Id = 133, Name = "Oakland Athletics", TotalPlayers = 38, Deleted = false },
                new Team { Id = 134, Name = "Pittsburgh Pirates", TotalPlayers = 36, Deleted = false },
                new Team { Id = 135, Name = "San Diego Padres", TotalPlayers = 34, Deleted = false },
                new Team { Id = 136, Name = "Seattle Mariners", TotalPlayers = 37, Deleted = false },
                new Team { Id = 137, Name = "San Francisco Giants", TotalPlayers = 40, Deleted = false },
                new Team { Id = 138, Name = "St. Louis Cardinals", TotalPlayers = 38, Deleted = false },
                new Team { Id = 139, Name = "Tampa Bay Rays", TotalPlayers = 40, Deleted = false },
                new Team { Id = 140, Name = "Texas Rangers", TotalPlayers = 40, Deleted = false },
                new Team { Id = 141, Name = "Toronto Blue Jays", TotalPlayers = 40, Deleted = false },
                new Team { Id = 142, Name = "Minnesota Twins", TotalPlayers = 38, Deleted = false },
                new Team { Id = 143, Name = "Philadelphia Phillies", TotalPlayers = 39, Deleted = false },
                new Team { Id = 144, Name = "Atlanta Braves", TotalPlayers = 38, Deleted = false },
                new Team { Id = 145, Name = "Chicago White Sox", TotalPlayers = 39, Deleted = false },
                new Team { Id = 146, Name = "Miami Marlins", TotalPlayers = 40, Deleted = false },
                new Team { Id = 147, Name = "New York Yankees", TotalPlayers = 33, Deleted = false },
                new Team { Id = 158, Name = "Milwaukee Brewers", TotalPlayers = 40, Deleted = false }
            );
        }

        // Tijdelijke functie om seed data te genereren voor SeedPlayers
        public void GeneratePlayerSeedData()
        {
            try
            {
                string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "players.csv");
                if (!File.Exists(csvPath))
                {
                    Console.WriteLine($"CSV file not found at: {csvPath}");
                    return;
                }

                string seedData = Utils.SeedDataGenerator.GeneratePlayerSeedData(csvPath);

                // Write to a file in the project directory
                string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "playerSeedData.txt");
                File.WriteAllText(outputPath, seedData);

                Console.WriteLine($"Seed data generated successfully and saved to: {outputPath}");
                Console.WriteLine($"First few lines of generated data:");
                Console.WriteLine(seedData.Split('\n').Take(5).Aggregate((a, b) => a + "\n" + b));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating seed data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        private void SeedPlayers(ModelBuilder modelBuilder)
        {
            string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "players.csv");
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV file not found at: {csvPath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",", // Adjust delimiter if necessary
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<Player>().ToList();

            // update DOB met een random datum tussen 1/1/2000 en 31/12/2006
            DateTime startDate = new DateTime(2000, 1, 1);
            DateTime endDate = new DateTime(2006, 12, 31);
            int rangeDays = (endDate - startDate).Days;
            var random = new Random();
            foreach (var record in records)
            {
                record.DOB = startDate.AddDays(random.Next(rangeDays));
            }

            // add records in Players tabel
            modelBuilder.Entity<Player>().HasData(records);
        }
        private void SeedGames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasData(
            new Game
            {
                Id = 1,
                User = "admin@testemail.com",
                HomeTeamId = 108,
                AwayTeamId = 118,
                GameDate = "2024-12-31",
                GameTime = "00:00",
                HomeStartingPitcherId = 543294,
                AwayStartingPitcherId = 672582,
                TotalInnings = 9,
                ErrorsAwayTeam = 0,
                ErrorsHomeTeam = 0,
                Finished = true,
                HitsAwayTeam = 4,
                HitsHomeTeam = 3,
                RunsAwayTeam = 0,
                RunsHomeTeam = 1
            });
        }

        private void SeedStatistics(ModelBuilder modelBuilder)
        {
            string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "Statistics.csv");
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV file not found at: {csvPath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",", // Adjust delimiter if necessary
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<ScoreStatistic>().ToList();

            modelBuilder.Entity<ScoreStatistic>().HasData(records);
        }

        private void SeedSeasons(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Season>().HasData(
            new Season
            {
                Id = 1,
				PlayerKey = 8,
				GamesPlayed = 3,
				Year = 2023,
				PlateAppearences = 7,
				HStrikeOuts = 2,
				Hits = 3,
				Singles = 1,
				Doubles = 2,
				Triples = 0,
				HomeRuns = 0,
				BaseOnBalls = 1,
				HitByPitch = 1,
				SacrificeFlies = 0,
				SacrificeHits = 0,
				CaughtStealing = 2,
				StolenBases = 4,
				Runs = 3,
				Errors = 1,
				DoublePlays = 0,
				TriplePlays = 0,
				PassedBalls = 0,
				PStrikeOuts = 0,
                Deleted = false
            },
            new Season
            {
                Id = 2,
				PlayerKey = 6,
				GamesPlayed = 4,
				Year = 2023,
				PlateAppearences = 7,
				HStrikeOuts = 4,
				Hits = 2,
				Singles = 1,
				Doubles = 2,
				Triples = 0,
				HomeRuns = 0,
				BaseOnBalls = 1,
				HitByPitch = 1,
				SacrificeFlies = 0,
				SacrificeHits = 0,
				CaughtStealing = 2,
				StolenBases = 1,
				Runs = 3,
				Errors = 1,
				DoublePlays = 0,
				TriplePlays = 0,
				PassedBalls = 0,
				PStrikeOuts = 0,
                Deleted = false
            });
        }
    }
}
