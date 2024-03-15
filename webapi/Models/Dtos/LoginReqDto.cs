using System.ComponentModel.DataAnnotations;

namespace webapi.Models.Dtos
{
    public class LoginReqDto
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
