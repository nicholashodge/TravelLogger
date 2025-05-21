using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models.DTOs;

public class UpvoteDTO {
    public int Id { get; set; }
    [Required] public int UserId { get; set; }
    public UserDTO User { get; set; }
    [Required] public int RecommendationId { get; set; }
    public RecommendationDTO Recommendation { get; set; }
}