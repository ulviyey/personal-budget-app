using System.ComponentModel.DataAnnotations;

namespace Cardify.Core.Models
{
    public class UserLoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
} 