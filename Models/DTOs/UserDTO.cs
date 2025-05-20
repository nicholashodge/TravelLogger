using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; }
    public string Name { get; set; }
    public List<LogDTO>? Logs { get; set; }
}