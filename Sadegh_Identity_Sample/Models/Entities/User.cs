using Microsoft.AspNetCore.Identity;

namespace Sadegh_Identity_Sample.Models.Entities
{
    public class User: IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}
