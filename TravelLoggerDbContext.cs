using Microsoft.EntityFrameworkCore;
using TravelLogger.Models;

public class TravelLoggerDbContext : DbContext
{

    // Define tables here
    
    public TravelLoggerDbContext(DbContextOptions<TravelLoggerDbContext> context) : base(context)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data here
        
    }
}