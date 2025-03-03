using System.ComponentModel.DataAnnotations;

namespace Sadegh_Identity_Sample.Models.Dto.Account
{
    public class TwoFactorLoginDto
    {
        [Required]
        public string Code { get; set; }
        public bool IsPersistent { get; set; }
        public string Provider { get; set; }
    }
}
