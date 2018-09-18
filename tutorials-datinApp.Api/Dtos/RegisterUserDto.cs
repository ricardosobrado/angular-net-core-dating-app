using System.ComponentModel.DataAnnotations;

namespace tutorials_datinApp.Api.Dtos
{
    public class RegisteredUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "The password must have between 4 and 8 characters")]
        public string Password { get; set; }
    }
}