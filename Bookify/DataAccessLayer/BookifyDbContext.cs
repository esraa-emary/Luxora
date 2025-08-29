using Bookify.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookify.DataAccessLayer
{
    public class BookifyDbContext : DbContext
    {
        public BookifyDbContext(DbContextOptions<BookifyDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.SSN).IsUnique();
            });

            // Configure RoomType entity
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.RoomTypeId);
                entity.Property(e => e.RoomTypeId).ValueGeneratedOnAdd();
            });

            // Configure Room entity
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomNumber);
                entity.HasOne(e => e.RoomType)
                      .WithMany(rt => rt.Rooms)
                      .HasForeignKey(e => e.RoomTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Reservation entity
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId);
                entity.Property(e => e.ReservationId).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Reservations)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Room)
                      .WithMany(r => r.Reservations)
                      .HasForeignKey(e => e.RoomNumber)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.PaymentId).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.Reservation)
                      .WithMany(r => r.Payments)
                      .HasForeignKey(e => e.ReservationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Feedback entity
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.FeedbackId);
                entity.Property(e => e.FeedbackId).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Feedbacks)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Room Types based on navigation menu
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { RoomTypeId = 1, TypeName = "Standard" },
                new RoomType { RoomTypeId = 2, TypeName = "Deluxe" },
                new RoomType { RoomTypeId = 3, TypeName = "Suite" },
                new RoomType { RoomTypeId = 4, TypeName = "Special View" },
                new RoomType { RoomTypeId = 5, TypeName = "Family & Group" }
            );

            // Seed some sample rooms
            modelBuilder.Entity<Room>().HasData(
                // Standard Rooms
                new Room { RoomNumber = 101, Description = "Comfortable standard room with city view", Status = "Available", Price = 150.00m, RoomTypeId = 1 },
                new Room { RoomNumber = 102, Description = "Standard room with modern amenities", Status = "Available", Price = 150.00m, RoomTypeId = 1 },
                new Room { RoomNumber = 103, Description = "Cozy standard room perfect for solo travelers", Status = "Available", Price = 150.00m, RoomTypeId = 1 },

                // Deluxe Rooms
                new Room { RoomNumber = 201, Description = "Spacious deluxe room with premium furnishing", Status = "Available", Price = 250.00m, RoomTypeId = 2 },
                new Room { RoomNumber = 202, Description = "Deluxe room with balcony and garden view", Status = "Available", Price = 250.00m, RoomTypeId = 2 },

                // Suite Rooms
                new Room { RoomNumber = 301, Description = "Luxurious suite with separate living area", Status = "Available", Price = 450.00m, RoomTypeId = 3 },
                new Room { RoomNumber = 302, Description = "Executive suite with work area and premium amenities", Status = "Available", Price = 450.00m, RoomTypeId = 3 },

                // Special View Rooms
                new Room { RoomNumber = 401, Description = "Ocean view room with panoramic windows", Status = "Available", Price = 350.00m, RoomTypeId = 4 },
                new Room { RoomNumber = 402, Description = "Mountain view room with scenic balcony", Status = "Available", Price = 350.00m, RoomTypeId = 4 },

                // Family & Group Rooms
                new Room { RoomNumber = 501, Description = "Family room with multiple beds, sleeps 6", Status = "Available", Price = 300.00m, RoomTypeId = 5 },
                new Room { RoomNumber = 502, Description = "Group suite with connecting rooms, sleeps 8", Status = "Available", Price = 400.00m, RoomTypeId = 5 }
            );
        }
    }
}
