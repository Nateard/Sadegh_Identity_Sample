using System.ComponentModel.DataAnnotations;

namespace Sadegh_Identity_Sample.Models.Dto.Account
{
    public class ForgetPasswordConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
