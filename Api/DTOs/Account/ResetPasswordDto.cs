using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "New Password must be at least {2} and max {1} characters")]
        public string NewPassword { get; set; }
    }
}
