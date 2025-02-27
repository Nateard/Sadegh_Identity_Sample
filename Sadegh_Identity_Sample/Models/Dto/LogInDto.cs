using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sadegh_Identity_Sample.Models.Dto
{
    public class LogInDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [DisplayName ("Remember Me")]
        public bool IsPersistent { get; set; } = false; 


        public string ReturnUrl { get; set; }
    }
}
