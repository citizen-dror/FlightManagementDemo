using FlightManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<PriceAlert> PriceAlerts { get; set; }
        public DbSet<UserFlight> UserFlights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.DeviceToken)
                    .HasMaxLength(255);
                entity.Property(e => e.MobileOS)
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Email)
                    .IsUnique();  // Ensures email is unique
                //foreign keys
                entity.HasMany(e => e.PriceAlerts)
                    .WithOne(pa => pa.User)
                    .HasForeignKey(pa => pa.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserFlights)
                    .WithOne(uf => uf.User)
                    .HasForeignKey(uf => uf.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Flight entity
            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FlightNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Airline)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.DepartureAirport)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.ArrivalAirport)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DepartureTime)
                    .IsRequired();

                entity.Property(e => e.ArrivalTime)
                    .IsRequired();

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasDefaultValue("USD");

                entity.HasIndex(e => e.FlightNumber)
                    .IsUnique();  // Ensures flight numbers are unique
                entity.HasIndex(e => e.DepartureTime);
                entity.HasIndex(e => e.ArrivalAirport);
            });

            // Configure UserFlight entity
            modelBuilder.Entity<UserFlight>(entity =>
            {
                // Define composite primary key
                entity.HasKey(e => new { e.UserId, e.FlightId });

                // Define relationships
                entity.HasOne(uf => uf.User)
                    .WithMany(u => u.UserFlights)
                    .HasForeignKey(uf => uf.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uf => uf.Flight)
                    .WithMany(f => f.UserFlights)
                    .HasForeignKey(uf => uf.FlightId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance
                entity.HasIndex(uf => uf.UserId);
                entity.HasIndex(uf => uf.FlightId);
            });

            // Configure PriceAlert entity
            modelBuilder.Entity<PriceAlert>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Origin)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TargetPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasDefaultValue("USD");

                entity.Property(e => e.IsFlexibleDate)
                    .IsRequired();

                entity.Property(e => e.FlexibleDays)
                    .IsRequired(false);

                entity.Property(e => e.OneWay)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                    .IsRequired();

                // Indexes for performance
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Origin);
                entity.HasIndex(e => e.Destination);
                entity.HasIndex(e => e.TargetPrice);
                entity.HasIndex(e => e.IsActive);
            });
        }

    }
}
