using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models;

public class Recommendation {
    public int Id { get; set; }
    [Required] public int CityId { get; set; }
    public City City { get; set; }
    public string Text { get; set; }
}