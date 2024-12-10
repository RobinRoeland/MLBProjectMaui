﻿// <auto-generated />
using System;
using BaseballCalcASP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BaseballCalcASP.Migrations
{
    [DbContext(typeof(BaseballCalcASPContext))]
    [Migration("20241127152919_AddStatisticScores")]
    partial class AddStatisticScores
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BaseballModelsLib.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("AwayStartingPitcherId")
                        .HasColumnType("int");

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("int");

                    b.Property<string>("GameDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HomeStartingPitcherId")
                        .HasColumnType("int");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Games");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AwayStartingPitcherId = 3,
                            AwayTeamId = 4,
                            GameDate = "2008-12-04",
                            GameTime = "13:00",
                            HomeStartingPitcherId = 1,
                            HomeTeamId = 1
                        });
                });

            modelBuilder.Entity("BaseballModelsLib.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("APILink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int?>("MLBPersonId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Rugnummer")
                        .HasColumnType("int");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Players");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DOB = new DateTime(2004, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Waldo",
                            TeamId = 1
                        },
                        new
                        {
                            Id = 2,
                            DOB = new DateTime(2004, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Waldo2",
                            TeamId = 1
                        },
                        new
                        {
                            Id = 3,
                            DOB = new DateTime(2003, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Kangoeroe",
                            TeamId = 4
                        },
                        new
                        {
                            Id = 4,
                            DOB = new DateTime(2006, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Test",
                            TeamId = 2
                        },
                        new
                        {
                            Id = 5,
                            DOB = new DateTime(2006, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Test2",
                            TeamId = 2
                        },
                        new
                        {
                            Id = 6,
                            DOB = new DateTime(2005, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "A1",
                            TeamId = 2
                        },
                        new
                        {
                            Id = 7,
                            DOB = new DateTime(2006, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "A2",
                            TeamId = 3
                        },
                        new
                        {
                            Id = 8,
                            DOB = new DateTime(2008, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "A3",
                            TeamId = 3
                        });
                });

            modelBuilder.Entity("BaseballModelsLib.Models.ScoreStatistic", b =>
                {
                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<int>("Inning")
                        .HasColumnType("int");

                    b.Property<int>("InningTop")
                        .HasColumnType("int");

                    b.Property<int>("PersonMLBId")
                        .HasColumnType("int");

                    b.Property<int>("ScoreIdInInning")
                        .HasColumnType("int");

                    b.HasKey("GameId", "Inning", "InningTop", "PersonMLBId", "ScoreIdInInning");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("BaseballModelsLib.Models.Season", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("BaseOnBalls")
                        .HasColumnType("int");

                    b.Property<int>("CaughtStealing")
                        .HasColumnType("int");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("DoublePlays")
                        .HasColumnType("int");

                    b.Property<int>("Doubles")
                        .HasColumnType("int");

                    b.Property<int>("Errors")
                        .HasColumnType("int");

                    b.Property<int>("GamesPlayed")
                        .HasColumnType("int");

                    b.Property<int>("HStrikeOuts")
                        .HasColumnType("int");

                    b.Property<int>("HitByPitch")
                        .HasColumnType("int");

                    b.Property<int>("Hits")
                        .HasColumnType("int");

                    b.Property<int>("HomeRuns")
                        .HasColumnType("int");

                    b.Property<int>("PStrikeOuts")
                        .HasColumnType("int");

                    b.Property<int>("PassedBalls")
                        .HasColumnType("int");

                    b.Property<int>("PlateAppearences")
                        .HasColumnType("int");

                    b.Property<int>("PlayerKey")
                        .HasColumnType("int");

                    b.Property<int>("Runs")
                        .HasColumnType("int");

                    b.Property<int>("SacrificeFlies")
                        .HasColumnType("int");

                    b.Property<int>("SacrificeHits")
                        .HasColumnType("int");

                    b.Property<int>("Singles")
                        .HasColumnType("int");

                    b.Property<int>("StolenBases")
                        .HasColumnType("int");

                    b.Property<int>("TriplePlays")
                        .HasColumnType("int");

                    b.Property<int>("Triples")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Seasons");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BaseOnBalls = 1,
                            CaughtStealing = 2,
                            Deleted = false,
                            DoublePlays = 0,
                            Doubles = 2,
                            Errors = 1,
                            GamesPlayed = 3,
                            HStrikeOuts = 2,
                            HitByPitch = 1,
                            Hits = 3,
                            HomeRuns = 0,
                            PStrikeOuts = 0,
                            PassedBalls = 0,
                            PlateAppearences = 7,
                            PlayerKey = 8,
                            Runs = 3,
                            SacrificeFlies = 0,
                            SacrificeHits = 0,
                            Singles = 1,
                            StolenBases = 4,
                            TriplePlays = 0,
                            Triples = 0,
                            Year = 2023
                        },
                        new
                        {
                            Id = 2,
                            BaseOnBalls = 1,
                            CaughtStealing = 2,
                            Deleted = false,
                            DoublePlays = 0,
                            Doubles = 2,
                            Errors = 1,
                            GamesPlayed = 4,
                            HStrikeOuts = 4,
                            HitByPitch = 1,
                            Hits = 2,
                            HomeRuns = 0,
                            PStrikeOuts = 0,
                            PassedBalls = 0,
                            PlateAppearences = 7,
                            PlayerKey = 6,
                            Runs = 3,
                            SacrificeFlies = 0,
                            SacrificeHits = 0,
                            Singles = 1,
                            StolenBases = 1,
                            TriplePlays = 0,
                            Triples = 0,
                            Year = 2023
                        });
                });

            modelBuilder.Entity("BaseballModelsLib.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("FranchiseCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LeagueName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MLB_Org_ID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameDisplayBrief")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalPlayers")
                        .HasColumnType("int");

                    b.Property<string>("VenueName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Teams");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Deleted = false,
                            Name = "Waldos",
                            TotalPlayers = 2
                        },
                        new
                        {
                            Id = 2,
                            Deleted = false,
                            Name = "Bebops",
                            TotalPlayers = 2
                        },
                        new
                        {
                            Id = 3,
                            Deleted = false,
                            Name = "Foxes",
                            TotalPlayers = 2
                        },
                        new
                        {
                            Id = 4,
                            Deleted = false,
                            Name = "Kangeroos",
                            TotalPlayers = 1
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "58308036-c9ee-45f7-ab08-ac79836c3837",
                            Name = "admin",
                            NormalizedName = "admin"
                        },
                        new
                        {
                            Id = "1397b04c-fc40-425e-a84d-7710747d6444",
                            Name = "user",
                            NormalizedName = "user"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator().HasValue("IdentityUser");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "896f9a66-8150-4203-bb63-8df62c2b0648",
                            RoleId = "58308036-c9ee-45f7-ab08-ac79836c3837"
                        },
                        new
                        {
                            UserId = "f44a23c4-f598-4d74-8f80-d1345fff8755",
                            RoleId = "1397b04c-fc40-425e-a84d-7710747d6444"
                        },
                        new
                        {
                            UserId = "7b67422e-23d4-42bf-8442-8e9b927e16d7",
                            RoleId = "1397b04c-fc40-425e-a84d-7710747d6444"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BaseballCalcASP.Models.AppUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("deleted")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("AppUser");

                    b.HasData(
                        new
                        {
                            Id = "896f9a66-8150-4203-bb63-8df62c2b0648",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "44a9100b-87d5-411d-a0a8-f5f0878ab195",
                            Email = "admin@testemail.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@TESTEMAIL.COM",
                            NormalizedUserName = "ADMIN@TESTEMAIL.COM",
                            PasswordHash = "AQAAAAIAAYagAAAAEEhF0CkB5rKTDdskkh1ZfSIOR05NvpGFwpf6Y0x90XfolUoJF5YVSwvb773InyXVTw==",
                            PhoneNumberConfirmed = true,
                            SecurityStamp = "b28c06ad-d4b4-415d-8936-433cda967001",
                            TwoFactorEnabled = false,
                            UserName = "admin@testemail.com",
                            FirstName = "System",
                            LastName = "Administrator",
                            deleted = false
                        },
                        new
                        {
                            Id = "f44a23c4-f598-4d74-8f80-d1345fff8755",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "447eac11-f28c-406f-b808-88bb468227cb",
                            Email = "user1@testemail.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "USER1@TESTEMAIL.COM",
                            NormalizedUserName = "USER1@TESTEMAIL.COM",
                            PasswordHash = "AQAAAAIAAYagAAAAECKi7ywaYQTTOCxFjuaLhvcGS3/GR5ZG/n2mU4N1HbSeHVyODWwpnQYtL8ToZvNffQ==",
                            PhoneNumberConfirmed = true,
                            SecurityStamp = "7825eba9-b9f4-4281-baae-3dc38f0f5e91",
                            TwoFactorEnabled = false,
                            UserName = "user1@testemail.com",
                            FirstName = "User1",
                            LastName = "AppUser1",
                            deleted = false
                        },
                        new
                        {
                            Id = "7b67422e-23d4-42bf-8442-8e9b927e16d7",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "347e4106-8ba4-4bc4-b60f-95a376c73cf3",
                            Email = "user2@testemail.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "USER2@TESTEMAIL.COM",
                            NormalizedUserName = "USER2@TESTEMAIL.COM",
                            PasswordHash = "AQAAAAIAAYagAAAAEGC9a3Jr+heSmDpzOIxiDF6XS4JSbORq4HTsvxfyMUzLC8fZf3tp3LI2scR5T2Totw==",
                            PhoneNumberConfirmed = true,
                            SecurityStamp = "7a54dd7d-373e-4b70-b23e-b2eb20a276c3",
                            TwoFactorEnabled = false,
                            UserName = "user2@testemail.com",
                            FirstName = "User2",
                            LastName = "AppUser2",
                            deleted = false
                        });
                });

            modelBuilder.Entity("BaseballModelsLib.Models.Player", b =>
                {
                    b.HasOne("BaseballModelsLib.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
