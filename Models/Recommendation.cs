using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models;

public class Recommendation {
    public int Id { get; set; }
    [Required] public int CityId { get; set; }
    [Required] public int UserId { get; set; }
    public City City { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
}