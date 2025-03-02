////using Sadegh_Identity_Sample.Models.Entities;

namespace Sadegh_Identity_Sample.Services.EmailServices
{
    public class EmailContentService
    {
        public EmailDto GenerateEmailConfirmationEmail(string callbackUrl)
        {
            var subject = "فعال سازی حساب کاربری ";
            var body = $"لطفا برای فعال سازی حساب کاربری بر روی لینک زیر کلیک کنید!  <br/> <a href={callbackUrl}> Link </a>";

            return new EmailDto()
            {
                Body = body,
                Subject = subject,
            };
        }
    }

}
