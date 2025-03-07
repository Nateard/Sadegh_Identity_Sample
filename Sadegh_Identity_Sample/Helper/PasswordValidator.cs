using Microsoft.AspNetCore.Identity;
using Sadegh_Identity_Sample.Models.Entities;

namespace Sadegh_Identity_Sample.Helper
{
    public class PasswordValidator : IPasswordValidator<User>
    {
        List<string> commonPassword = new List<string>()
        {
            "123456","password", "12345678","qwerty"
        };
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
        {
            if (commonPassword.Contains(password))
            {
                var result = IdentityResult.Failed(new IdentityError
                {
                    Code = "CommonPassword",
                    Description = "پسورد شما قابل شناسایی توسط ربات های هکر است! لطفا یک پسورد قوی انتخاب کنید",
                });
                return Task.FromResult(result);
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
