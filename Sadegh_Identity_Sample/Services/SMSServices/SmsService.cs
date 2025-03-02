using System.Net;

namespace Sadegh_Identity_Sample.Services.SMSServices
{
    public class SmsService
    {
        public void Send(string phoneNumber, string code)
        {
            var client = new WebClient();
            string url = $"https://api.kavenegar.com/v1/4F5A386C6C7765746A57474D2B30485A6C584B7462397639336C7742617044506F2F79374A6975767848773D/verify/lookup.json?receptor={phoneNumber}&token={code}&template=VerifyKarenStoreAccount";
            var content = client.DownloadString(url);

        }
    }
}
