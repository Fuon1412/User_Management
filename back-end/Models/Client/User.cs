using Microsoft.AspNetCore.Identity;

namespace back_end.Models.Client
{
    public class User : IdentityUser
    {
        public required string Username{get;set;}
        public string Password { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public UserInfor? UserInfor { get; set; }
    }
}