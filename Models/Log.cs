using System.ComponentModel.DataAnnotations;

namespace TravelLogger.Models;

public class Log {
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public DateTime LoggedTime { get; set; }
}