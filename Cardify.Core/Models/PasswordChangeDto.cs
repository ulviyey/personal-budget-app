using System.ComponentModel.DataAnnotations;

namespace Cardify.Core.Models
{
    public class PasswordChangeDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }
} 