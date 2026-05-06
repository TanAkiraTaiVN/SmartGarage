using Microsoft.EntityFrameworkCore;
using SmartGarage.Core.Entities;

namespace SmartGarage.Infrastructure.Data;

public class SmartGarageDbContext : DbContext
{
    public SmartGarageDbContext(DbContextOptions<SmartGarageDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ParkingSpot> ParkingSpots => Set<ParkingSpot>();
    public DbSet<ParkingTicket> ParkingTickets => Set<ParkingTicket>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ParkingRate> ParkingRates => Set<ParkingRate>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasIndex(e => e.LicensePlate).IsUnique();
            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(30);
            entity.HasOne(e => e.Owner)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ParkingSpot>(entity =>
        {
            entity.HasIndex(e => e.SpotCode).IsUnique();
            entity.Property(e => e.SpotCode).HasMaxLength(20);
            entity.Property(e => e.Zone).HasMaxLength(10);
        });

        modelBuilder.Entity<ParkingTicket>(entity =>
        {
            entity.HasIndex(e => e.TicketCode).IsUnique();
            entity.Property(e => e.TicketCode).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.ParkingTickets)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ParkingSpot)
                .WithMany(s => s.ParkingTickets)
                .HasForeignKey(e => e.ParkingSpotId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User)
                .WithMany(u => u.ParkingTickets)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.TransactionCode).IsUnique();
            entity.Property(e => e.TransactionCode).HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.ParkingTicket)
                .WithOne(t => t.Payment)
                .HasForeignKey<Payment>(e => e.ParkingTicketId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ParkingRate>(entity =>
        {
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DailyRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MonthlyRate).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParkingRate>().HasData(
            new ParkingRate
            {
                Id = 1,
                VehicleType = Core.Enums.VehicleType.Motorcycle,
                HourlyRate = 5000,
                DailyRate = 30000,
                MonthlyRate = 500000,
                IsActive = true,
                EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ParkingRate
            {
                Id = 2,
                VehicleType = Core.Enums.VehicleType.Car,
                HourlyRate = 20000,
                DailyRate = 100000,
                MonthlyRate = 2000000,
                IsActive = true,
                EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ParkingRate
            {
                Id = 3,
                VehicleType = Core.Enums.VehicleType.Bicycle,
                HourlyRate = 2000,
                DailyRate = 10000,
                MonthlyRate = 200000,
                IsActive = true,
                EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ParkingRate
            {
                Id = 4,
                VehicleType = Core.Enums.VehicleType.Truck,
                HourlyRate = 30000,
                DailyRate = 150000,
                MonthlyRate = 3000000,
                IsActive = true,
                EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Admin",
                Email = "admin@smartgarage.com",
                Phone = "0901234567",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = Core.Enums.UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<ParkingSpot>().HasData(
            GenerateParkingSpots()
        );
    }

    private static ParkingSpot[] GenerateParkingSpots()
    {
        var spots = new List<ParkingSpot>();
        int id = 1;

        for (int floor = 1; floor <= 3; floor++)
        {
            for (int i = 1; i <= 10; i++)
            {
                spots.Add(new ParkingSpot
                {
                    Id = id++,
                    SpotCode = $"A{floor}{i:D2}",
                    Zone = "A",
                    Floor = floor,
                    SpotType = Core.Enums.VehicleType.Car,
                    Status = Core.Enums.ParkingSpotStatus.Available,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }

            for (int i = 1; i <= 20; i++)
            {
                spots.Add(new ParkingSpot
                {
                    Id = id++,
                    SpotCode = $"B{floor}{i:D2}",
                    Zone = "B",
                    Floor = floor,
                    SpotType = Core.Enums.VehicleType.Motorcycle,
                    Status = Core.Enums.ParkingSpotStatus.Available,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }
        }

        return spots.ToArray();
    }
}
