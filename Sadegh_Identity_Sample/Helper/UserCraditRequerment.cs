using Microsoft.AspNetCore.Authorization;

namespace Sadegh_Identity_Sample.Helpers
{
    public class UserCreditRequirement : IAuthorizationRequirement
    {
        public int Credit { get; set; }
        public UserCreditRequirement(int credit)
        {
            Credit = credit;
        }

    }

    public class UserCreditHandler : AuthorizationHandler<UserCreditRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserCreditRequirement requirement)
        {
            var claim = context.User.FindFirst("Credit");
            if (claim != null)
            {
                int credit = int.Parse(claim?.Value);
                if (credit >= requirement.Credit)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
