using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models.DTOs;

public class CityDTO
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } 
    public List<LogDTO>? Logs { get; set; }
}