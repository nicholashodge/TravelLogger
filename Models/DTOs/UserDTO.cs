using System.ComponentModel.DataAnnotations;
namespace TravelLogger.Models.DTOs;

public class UserDTO {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}