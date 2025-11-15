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
        public DbSet<CartItem> CartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.SSN).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure RoomType entity
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.RoomTypeId);
                entity.Property(e => e.RoomTypeId).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Room)
                      .WithMany(r => r.CartItems)
                      .HasForeignKey(e => e.RoomNumber)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // Configure Room entity
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomNumber);
                entity.Property(e => e.RoomNumber).ValueGeneratedNever(); // Room number is manually set
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
            // ===== Room Types =====
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { RoomTypeId = 1, TypeName = "rooms" },
                new RoomType { RoomTypeId = 2, TypeName = "dining" },
                new RoomType { RoomTypeId = 3, TypeName = "pool" },
                new RoomType { RoomTypeId = 4, TypeName = "events" },
                new RoomType { RoomTypeId = 5, TypeName = "lobby" }
            );

            // ===== Rooms =====
            modelBuilder.Entity<Room>().HasData(
                // ----- Rooms category -----
                new Room
                {
                    RoomNumber = 1,
                    RoomName = "Presidential Suite",
                    Description = "Experience ultimate luxury with panoramic city views, a spacious living area, dining room, and premium amenities.",
                    Status = "Available",
                    Price = 899.00m,
                    RoomTypeId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?auto=format&fit=crop&w=600&q=80",
                    Capacity = 4,
                    Size = "120",
                    BedType = "King Size × 2",
                    View = "Panoramic City View",
                    Amenities = "WiFi,Air Conditioning,Mini Bar,Jacuzzi,Smart TV,Room Service,Balcony,Safe,Butler Service,Private Check-in"
                },
                new Room
                {
                    RoomNumber = 2,
                    RoomName = "Deluxe King Room",
                    Description = "Elegant room with premium amenities and modern design for a comfortable stay.",
                    Status = "Available",
                    Price = 120.00m,
                    RoomTypeId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=600&q=80",
                    Capacity = 2,
                    Size = "50",
                    BedType = "King Size",
                    View = "City View",
                    Amenities = "WiFi,Smart TV,Mini Bar,Air Conditioning,Room Service,Safe"
                },
                new Room
                {
                    RoomNumber = 3,
                    RoomName = "Luxury Bathroom",
                    Description = "Marble bathroom with deep soaking tub, rain shower, and luxurious decor.",
                    Status = "Available",
                    Price = 90.00m,
                    RoomTypeId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=600&q=80",
                    Capacity = 1,
                    Size = "25",
                    BedType = "None",
                    View = "Interior",
                    Amenities = "Jacuzzi,Hair Dryer,Toiletries,Towels"
                },

                // ----- Dining category -----
                new Room
                {
                    RoomNumber = 4,
                    RoomName = "Main Restaurant",
                    Description = "Fine dining experience with gourmet cuisine and elegant atmosphere.",
                    Status = "Available",
                    Price = 200.00m,
                    RoomTypeId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=600&q=80",
                    Capacity = 60,
                    Size = "500",
                    BedType = "-",
                    View = "Garden View",
                    Amenities = "WiFi,Air Conditioning,Private Dining,Bar,Music System"
                },
                new Room
                {
                    RoomNumber = 5,
                    RoomName = "Gourmet Cuisine",
                    Description = "Exquisite dishes prepared by internationally acclaimed chefs.",
                    Status = "Available",
                    Price = 180.00m,
                    RoomTypeId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=600&q=80",
                    Capacity = 80,
                    Size = "450",
                    BedType = "-",
                    View = "Open Kitchen View",
                    Amenities = "WiFi,Bar,Chef’s Table,Music System,Private Dining"
                },
                new Room
                {
                    RoomNumber = 6,
                    RoomName = "Skyline Bar",
                    Description = "Rooftop bar with signature cocktails and breathtaking skyline views.",
                    Status = "Available",
                    Price = 160.00m,
                    RoomTypeId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=600&q=80",
                    Capacity = 100,
                    Size = "550",
                    BedType = "-",
                    View = "City Skyline",
                    Amenities = "WiFi,Bar,Lounge Music,Outdoor Seating,VIP Section"
                },

                // ----- Pool category -----
                new Room
                {
                    RoomNumber = 7,
                    RoomName = "Infinity Pool",
                    Description = "Stunning infinity pool with panoramic views of the city skyline.",
                    Status = "Available",
                    Price = 220.00m,
                    RoomTypeId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1561501878-aabd62634533?auto=format&fit=crop&w=600&q=80",
                    Capacity = 50,
                    Size = "600",
                    BedType = "-",
                    View = "Skyline View",
                    Amenities = "Pool Bar,Sun Loungers,Jacuzzi,Towel Service,WiFi"
                },
                new Room
                {
                    RoomNumber = 8,
                    RoomName = "Luxury Spa",
                    Description = "Relaxing spa treatments and therapies for ultimate wellness.",
                    Status = "Available",
                    Price = 250.00m,
                    RoomTypeId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1540497077202-7c8a3999166f?auto=format&fit=crop&w=600&q=80",
                    Capacity = 20,
                    Size = "400",
                    BedType = "-",
                    View = "Poolside",
                    Amenities = "Massage Rooms,Steam Room,Sauna,Jacuzzi,Relaxation Lounge"
                },

                // ----- Events category -----
                new Room
                {
                    RoomNumber = 9,
                    RoomName = "Wedding Venue",
                    Description = "Elegant space for unforgettable weddings and celebrations.",
                    Status = "Available",
                    Price = 300.00m,
                    RoomTypeId = 4,
                    ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=600&q=80",
                    Capacity = 200,
                    Size = "900",
                    BedType = "-",
                    View = "Garden View",
                    Amenities = "Stage,Sound System,Lighting,WiFi,Bridal Suite"
                },
                new Room
                {
                    RoomNumber = 10,
                    RoomName = "Conference Room",
                    Description = "State-of-the-art meeting facilities with modern technology.",
                    Status = "Available",
                    Price = 180.00m,
                    RoomTypeId = 4,
                    ImageUrl = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=600&q=80",
                    Capacity = 50,
                    Size = "300",
                    BedType = "-",
                    View = "City View",
                    Amenities = "Projector,WiFi,Whiteboard,Conference Table,Sound System"
                },

                // ----- Lobby category -----
                new Room
                {
                    RoomNumber = 11,
                    RoomName = "Grand Lobby",
                    Description = "Impressive entrance with elegant decor and luxurious ambiance.",
                    Status = "Available",
                    Price = 200.00m,
                    RoomTypeId = 5,
                    ImageUrl = "https://images.unsplash.com/photo-1564501049412-61c2a3083791?auto=format&fit=crop&w=600&q=80",
                    Capacity = 100,
                    Size = "700",
                    BedType = "-",
                    View = "Main Entrance",
                    Amenities = "Reception,Concierge,Seating Area,Art Displays,WiFi"
                },
                new Room
                {
                    RoomNumber = 12,
                    RoomName = "Executive Lounge",
                    Description = "Exclusive lounge for premium guests with VIP services.",
                    Status = "Available",
                    Price = 220.00m,
                    RoomTypeId = 5,
                    ImageUrl = "https://images.unsplash.com/photo-1535827841776-24afc1e255ac?auto=format&fit=crop&w=600&q=80",
                    Capacity = 40,
                    Size ="250",
                    BedType = "-",
                    View = "Lobby View",
                    Amenities = "WiFi,Bar,Snacks,Lounge Seating,Newspapers,Coffee Machine"
                }
            );
        }
    }
}
