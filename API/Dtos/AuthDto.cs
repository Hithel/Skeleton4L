using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class AuthDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Code { get; set; }

}