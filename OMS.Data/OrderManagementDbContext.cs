using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Models;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace OMS.Data
{
    public class OrderManagementDbContext : DbContext
    {
        protected readonly IConfiguration? Configuration;

        public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options)
            : base(options)
        { }

        public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }


        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<ClosedTrade> ClosedTrades { get; set; }
        public DbSet<LiveOrder> LiveOrder { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<OrderFlow> OrderFlow { get; set; }
        public DbSet<ScoreCard> ScoreCard { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    UserID = 999999,
                    DisplayName = "User1",
                    UserPwd = "abc",
                    FirstName = "User",
                    LastName = "One",
                    Email = "admin",
                    DateRegistered = DateTime.Now,
                    Enabled = 1,
                    EnabledLive = 0
                }); 

            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    UserID = 999998,
                    DisplayName = "User2",
                    UserPwd = "abc",
                    FirstName = "User",
                    LastName = "One",
                    Email = "admin",
                    DateRegistered = DateTime.Now,
                    Enabled = 1,
                    EnabledLive = 0
                }); 


        }


    }
}
