using Microsoft.EntityFrameworkCore;
using TravelLogger.Models;

public class TravelLoggerDbContext : DbContext
{

    // Define tables here

    public DbSet<City> Cities { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    public DbSet<Upvote> Upvotes { get; set; }
    public DbSet<User> Users { get; set; }

    public TravelLoggerDbContext(DbContextOptions<TravelLoggerDbContext> context) : base(context)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data here
        modelBuilder.Entity<City>().HasData(new City[]
        {
            new City { Id = 1, Name = "London" },
            new City { Id = 2, Name = "Paris" },
            new City { Id = 3, Name = "Tokyo" },
            new City { Id = 4, Name = "New York" },
            new City { Id = 5, Name = "Sydney" },
            new City { Id = 6, Name = "Rome" },
            new City { Id = 7, Name = "Berlin" }
        }
        );

        modelBuilder.Entity<User>().HasData(new User[]
            {
            new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" },
            new User { Id = 3, Name = "David Lee", Email = "david.lee@example.com" },
            new User { Id = 4, Name = "Emily Chen", Email = "emily.chen@example.com" },
            new User { Id = 5, Name = "Michael Brown", Email = "michael.brown@example.com" },
            new User { Id = 6, Name = "Jessica Davis", Email = "jessica.davis@example.com" },
            new User { Id = 7, Name = "Kevin Wilson", Email = "kevin.wilson@example.com" }
        }
        );

        modelBuilder.Entity<Log>().HasData(new Log[]
            {
            new Log { Id = 1, UserId = 1, CityId = 1, LoggedTime = DateTime.Now },
            new Log { Id = 2, UserId = 2, CityId = 2, LoggedTime = DateTime.Now },
            new Log { Id = 3, UserId = 3, CityId = 3, LoggedTime = DateTime.Now },
            new Log { Id = 4, UserId = 4, CityId = 4, LoggedTime = DateTime.Now },
            new Log { Id = 5, UserId = 5, CityId = 5, LoggedTime = DateTime.Now },
            new Log { Id = 6, UserId = 6, CityId = 6, LoggedTime = DateTime.Now },
            new Log { Id = 7, UserId = 7, CityId = 7, LoggedTime = DateTime.Now }
        }
        );

        modelBuilder.Entity<Recommendation>().HasData(new Recommendation[]
            {
            new Recommendation { Id = 1, CityId = 1, UserId = 1, Text = "Visit Buckingham Palace" },
            new Recommendation { Id = 2, CityId = 2, UserId = 2, Text = "See the Eiffel Tower" },
            new Recommendation { Id = 3, CityId = 3, UserId = 3, Text = "Explore the Shibuya Crossing" },
            new Recommendation { Id = 4, CityId = 4, UserId = 4, Text = "Walk through Central Park" },
            new Recommendation { Id = 5, CityId = 5, UserId = 5, Text = "Climb the Sydney Harbour Bridge" },
            new Recommendation { Id = 6, CityId = 6, UserId = 6, Text = "Visit the Colosseum" },
            new Recommendation { Id = 7, CityId = 7, UserId = 7, Text = "See the Brandenburg Gate" }
        }
        );

        modelBuilder.Entity<Upvote>().HasData(new Upvote[]
            {
            new Upvote { Id = 1, UserId = 1, RecommendationId = 1 },
            new Upvote { Id = 2, UserId = 2, RecommendationId = 2 },
            new Upvote { Id = 3, UserId = 3, RecommendationId = 3 },
            new Upvote { Id = 4, UserId = 4, RecommendationId = 4 },
            new Upvote { Id = 5, UserId = 5, RecommendationId = 5 },
            new Upvote { Id = 6, UserId = 6, RecommendationId = 6 },
            new Upvote { Id = 7, UserId = 7, RecommendationId = 7 }
        }
        );
    }
}
