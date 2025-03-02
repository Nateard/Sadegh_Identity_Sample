using System.ComponentModel.DataAnnotations;

namespace Sadegh_Identity_Sample.Models.Dto.Account
{
    public class SetPhoneNumberDto
    {
        [Required]
        [RegularExpression(@"(\+98|0)?9\d{9}")]
        public string PhoneNumber { get; set; }
    }
}
