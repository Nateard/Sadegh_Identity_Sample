using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Sadegh_Identity_Sample.Models.Entities;

namespace Sadegh_Identity_Sample.Data
{
    public class DataBaseContext : IdentityDbContext<User, Role , string>
    {
        //https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.EntityFramework/IdentityDbContext.cs
        public DataBaseContext(DbContextOptions <DataBaseContext> options)
            : base(options) 
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserLogin<string>>().HasKey(p=> new {p.ProviderKey , p.LoginProvider});
            builder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            builder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider,p.Name });
            ////builder.Entity<User>().Ignore(p => p.NormalizedEmail);
        }
    }
}
