using Microsoft.AspNetCore.Identity;

namespace Sadegh_Identity_Sample.Models.Entities
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
