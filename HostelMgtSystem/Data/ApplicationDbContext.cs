using HostelMgtSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HostelMgtSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hostel> Hostels { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomProp> RoomProps { get; set; }
        public DbSet<User> Users { get; set; }  //maps User model to the database
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; } 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hostel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HostelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Hostel)
                .WithMany()
                .HasForeignKey(r => r.HostelId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Room)
                .WithMany()
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.NoAction);

            // --- Seed Hostels ---
            modelBuilder.Entity<Hostel>().HasData(
                new Hostel { Id = 1, Name = "Pod Living Male Hostel", Address = "Campus" },
                new Hostel { Id = 2, Name = "Coorporative Queens Hostel", Address = "Campus" },
                new Hostel { Id = 3, Name = "Faith Male Hostel", Address = "University" },
                new Hostel { Id = 4, Name = "Entreprise Male Hostel", Address = "University" },
                new Hostel { Id = 5, Name = "Emerald Male Hostel", Address = "University" },
                new Hostel { Id = 6, Name = "Amethyst Male Hostel", Address = "University" },
                new Hostel { Id = 7, Name = "Trinity Female Hostel", Address = "University" },
                new Hostel { Id = 8, Name = "Redwood Female Hostel", Address = "University" },
                new Hostel { Id = 9, Name = "Delano Apartment", Address = "Campus Drive" }
            );

            // --- Seed Rooms ---
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 101, RoomNumber = "101", Type = "Single", IsAvailable = true, HostelId = 1 },
                new Room { Id = 102, RoomNumber = "102", Type = "Double", IsAvailable = true, HostelId = 1 },
                new Room { Id = 201, RoomNumber = "201", Type = "Single", IsAvailable = true, HostelId = 2 },
                new Room { Id = 202, RoomNumber = "202", Type = "Double", IsAvailable = false, HostelId = 2 },
                new Room { Id = 301, RoomNumber = "301", Type = "Single", IsAvailable = true, HostelId = 3 },
                new Room { Id = 302, RoomNumber = "302", Type = "Double", IsAvailable = true, HostelId = 3 },
                new Room { Id = 303, RoomNumber = "303", Type = "Single", IsAvailable = false, HostelId = 3 },
                new Room { Id = 401, RoomNumber = "401", Type = "Single", IsAvailable = true, HostelId = 4 },
                new Room { Id = 402, RoomNumber = "402", Type = "Double", IsAvailable = true, HostelId = 4 },
                new Room { Id = 403, RoomNumber = "403", Type = "Single", IsAvailable = true, HostelId = 4 },
                new Room { Id = 501, RoomNumber = "501", Type = "Double", IsAvailable = true, HostelId = 5 },
                new Room { Id = 502, RoomNumber = "502", Type = "Single", IsAvailable = false, HostelId = 5 },
                new Room { Id = 503, RoomNumber = "503", Type = "Double", IsAvailable = true, HostelId = 5 },
                new Room { Id = 601, RoomNumber = "601", Type = "Single", IsAvailable = true, HostelId = 6 },
                new Room { Id = 602, RoomNumber = "602", Type = "Double", IsAvailable = true, HostelId = 6 },
                new Room { Id = 603, RoomNumber = "603", Type = "Single", IsAvailable = false, HostelId = 6 },
                new Room { Id = 701, RoomNumber = "701", Type = "Double", IsAvailable = true, HostelId = 7 },
                new Room { Id = 702, RoomNumber = "702", Type = "Single", IsAvailable = true, HostelId = 7 },
                new Room { Id = 703, RoomNumber = "703", Type = "Double", IsAvailable = false, HostelId = 7 },
                new Room { Id = 801, RoomNumber = "801", Type = "Single", IsAvailable = true, HostelId = 8 },
                new Room { Id = 802, RoomNumber = "802", Type = "Double", IsAvailable = true, HostelId = 8 },
                new Room { Id = 803, RoomNumber = "803", Type = "Single", IsAvailable = true, HostelId = 8 },
                new Room { Id = 901, RoomNumber = "901", Type = "Double", IsAvailable = true, HostelId = 9 },
                new Room { Id = 902, RoomNumber = "902", Type = "Single", IsAvailable = false, HostelId = 9 },
                new Room { Id = 903, RoomNumber = "903", Type = "Double", IsAvailable = true, HostelId = 9 }
            );

            // --- Seed Initial Users directly into the database ---
            
            modelBuilder.Entity<User>().HasData(
                // Admins
                new User { Id = 1, Email = "daniel@gmail.com", PasswordHash = "$2a$11$y8HJdlnpn0MoWczzU1E5SeVRPFIp9TyAYefFHyrTw8dvIZMEvrlM2", Role = "Admin", Name = "Daniel Ezinna", PhoneNumber = "08011110001", Gender = "Male", Level = "N/A", UniqueId = "ADM002" }, // Password: 3088
                new User { Id = 2, Email = "emmanuel@gmail.com", PasswordHash = "$2a$11$Y39na7fpcNbJskfvjtqPmucmwIYUNQfD8oTcNealyeSeCM9B1DOCy", Role = "Admin", Name = "Emmanuel Paul", PhoneNumber = "08011110002", Gender = "Male", Level = "N/A", UniqueId = "ADM003" }, // Password: 2902

                // Users
                new User { Id = 3, Email = "joe@gmail.com", PasswordHash = "$2a$11$CksL7hGZv9F3Y3flF6JRTu21cP2qJctSRiZzPjbDrIEUIsOQFffYq", Role = "User", Name = "Joe Michaels", PhoneNumber = "08122220001", Gender = "Male", Level = "100 Level", UniqueId = "MAT1002" }, // Password: 6430
                new User { Id = 4, Email = "mark@gmail.com", PasswordHash = "$2a$11$vqraBVVkMKVYE.Jl4bGlYOKtqF8dzFy7WBo2ufaLfxDnxzL5wTNV.", Role = "User", Name = "Mark Spencer", PhoneNumber = "08122220002", Gender = "Male", Level = "200 Level", UniqueId = "MAT2003" }, // Password: 3721
                new User { Id = 5, Email = "prisca@gmail.com", PasswordHash = "$2a$11$BTiBkEzPeHiiBbu4jj1hJe1oopiUDEgBtFOUuwz64FEpxyHB/iQya", Role = "User", Name = "Prisca Daniels", PhoneNumber = "08122220003", Gender = "Female", Level = "100 Level", UniqueId = "MAT1004" }, // Password: 2113
                new User { Id = 6, Email = "yamal@gmail.com", PasswordHash = "$2a$11$JplU.flbRIsBX71wk/o99OuNnISC/7qcDt6hN6yY.3cxi5xXHaPP2", Role = "User", Name = "Yamal Lamine", PhoneNumber = "08122220004", Gender = "Male", Level = "300 Level", UniqueId = "MAT3005" }, // Password: 7288
                new User { Id = 7, Email = "jim@gmail.com", PasswordHash = "$2a$11$2XpJloN2GhqZ5R7iRrb0MO7UCajglkAEkY4LcQTEKAE3bHsH0Jtk2", Role = "User", Name = "Jim Ray", PhoneNumber = "08122220005", Gender = "Male", Level = "200 Level", UniqueId = "MAT2006" }, // Password: 4119
                new User { Id = 8, Email = "gerald@gmail.com", PasswordHash = "$2a$11$J0YG8eG7S5p780TBKZZzSeYvX9LKa0JPgMX521.sjfZLyTGcirDpi", Role = "User", Name = "Gerald Cruz", PhoneNumber = "08122220006", Gender = "Male", Level = "400 Level", UniqueId = "MAT4007" }, // Password: 5444
                new User { Id = 9, Email = "susan@gmail.com", PasswordHash = "$2a$11$4Y5pooeTU/mM5QW56ZDYye2IO6lkEEHMtPthmRPswZWb5Uibuy.Ia", Role = "User", Name = "Susan Akintola", PhoneNumber = "08033330001", Gender = "Female", Level = "100 Level", UniqueId = "MAT1008" }, // Password: 8368
                new User { Id = 10, Email = "grace@gmail.com", PasswordHash = "$2a$11$xo2vB3lunFpILD48HmSbjeVm3CLGmnYK7qWMlwXko5oji0oJxp9Ea", Role = "User", Name = "Grace Obi", PhoneNumber = "08033330002", Gender = "Female", Level = "200 Level", UniqueId = "MAT2009" }, // Password: 7075
                new User { Id = 11, Email = "emma@gmail.com", PasswordHash = "$2a$11$nge9HgCuCe0UB.8ehP.da.VO0IvgvuSb/wzhHqw3t/5.gtkvMPYE.", Role = "User", Name = "Emma Okeke", PhoneNumber = "08033330003", Gender = "Female", Level = "300 Level", UniqueId = "MAT3010" }, // Password: 2059
                new User { Id = 12, Email = "chinedu@gmail.com", PasswordHash = "$2a$11$2/hH6I9Ndj22NF/qloyZ3.PVo5245MdR0rFGVrZ29tcNW5y50VkL2", Role = "User", Name = "Chinedu Nwosu", PhoneNumber = "08033330004", Gender = "Male", Level = "400 Level", UniqueId = "MAT4011" }, // Password: 9781
                new User { Id = 13, Email = "vivian@gmail.com", PasswordHash = "$2a$11$ukDFGNInIZKjbQRy6ya8SOWShTqxZ8OddeqXJlm/wZPCB5SCOC.Uu", Role = "User", Name = "Vivian Eze", PhoneNumber = "08033330005", Gender = "Female", Level = "200 Level", UniqueId = "MAT2012" }, // Password: 0665
                new User { Id = 14, Email = "kenneth@gmail.com", PasswordHash = "$2a$11$YHtM9X8QI5PH0k0HF0Otue6ODvCgAO5hhaGeWzRbdJTaNHjBZXypq", Role = "User", Name = "Kenneth Obi", PhoneNumber = "08033330006", Gender = "Male", Level = "300 Level", UniqueId = "MAT3013" }, // Password: 9488
                new User { Id = 15, Email = "linda@gmail.com", PasswordHash = "$2a$11$LIPZ0y9fksODRYVPQg2UjusAkzV2vE12xBdW/nDu0284t2kfENbWa", Role = "User", Name = "Linda George", PhoneNumber = "08033330007", Gender = "Female", Level = "100 Level", UniqueId = "MAT1014" }, // Password: 6602
                new User { Id = 16, Email = "hassan@gmail.com", PasswordHash = "$2a$11$ZPHhMVNLS68sE8RugjnttuWJ9P4BT4x9C.EWfM6lOJihoi7ZduhZK", Role = "User", Name = "Hassan Bello", PhoneNumber = "08033330008", Gender = "Male", Level = "100 Level", UniqueId = "MAT1015" }, // Password: 9502
                new User { Id = 17, Email = "blessing@gmail.com", PasswordHash = "$2a$11$mTsRFsI/ExSwnRRtAgT2XuVAfE5ETLUGulTeCQlvgTZaxNHWV1jva", Role = "User", Name = "Blessing Ojo", PhoneNumber = "08033330009", Gender = "Female", Level = "400 Level", UniqueId = "MAT4016" }, // Password: 8449
                new User { Id = 18, Email = "femi@gmail.com", PasswordHash = "$2a$11$a4Vq6Mlwm33Y6uvEvx2oZ..ftjoJRQ97RZ2VVTeQHGkCf4MdLm1VG", Role = "User", Name = "Femi Adebayo", PhoneNumber = "08033330010", Gender = "Male", Level = "200 Level", UniqueId = "MAT2017" }, // Password: 6615
                new User { Id = 19, Email = "mary@gmail.com", PasswordHash = "$2a$11$Ukpu3jNR28.fZTiPDbmNd.2iDV4P8p92zWPCPshcNm/dM4OlQRtMC", Role = "User", Name = "Mary Akpan", PhoneNumber = "08033330011", Gender = "Female", Level = "300 Level", UniqueId = "MAT3018" }, // Password: 8026
                new User { Id = 20, Email = "uche@gmail.com", PasswordHash = "$2a$11$ZbjmjuxpwBJqi70Oz2AwteLF.4o9BLh7iouW3eTh3p.U3ILo0prma", Role = "User", Name = "Uche Okoro", PhoneNumber = "08033330012", Gender = "Male", Level = "100 Level", UniqueId = "MAT1019" }  // Password: 2667
            );
        }
    }
}
