using System.ComponentModel.DataAnnotations;

namespace TravelLogger.Models.DTOs;

public class LogDTO {
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    public UserDTO User { get; set; }
    public int CityId { get; set; }
    public CityDTO City { get; set; }
    public DateTime LoggedTime { get; set; }
}