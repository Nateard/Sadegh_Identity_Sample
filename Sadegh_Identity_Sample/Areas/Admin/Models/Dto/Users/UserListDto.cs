namespace Sadegh_Identity_Sample.Areas.Admin.Models.Dto.Users
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
