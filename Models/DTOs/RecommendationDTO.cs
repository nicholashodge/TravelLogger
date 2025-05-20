using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models.DTOs;

public class RecommendationDTO {
    public int Id { get; set; }
    [Required]
    public int CityId { get; set; }
    public CityDTO City { get; set; }
    public string Text { get; set; }
}