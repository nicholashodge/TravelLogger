using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models;

public class City {
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public List<Log>? Logs { get; set; }
    public Recommendation? Recommendation { get; set; }
}