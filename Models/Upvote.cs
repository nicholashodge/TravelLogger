using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models;

public class Upvote {
    public int Id { get; set; }
    [Required] public int UserId { get; set; }
    public User User { get; set; }
    [Required] public int RecommendationId { get; set; }
    public Recommendation Recommendation { get; set; }
}