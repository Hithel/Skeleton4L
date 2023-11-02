

using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace API.Dtos;
public class RegisterDto : BaseEntity
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public DateTime DateCreated { get; set; } =  DateTime.Now;
}