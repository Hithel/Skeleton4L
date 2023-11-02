
using Domain.Entities;

namespace API.Dtos;
    public class UserDto : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; } =  DateTime.Now;
    }
