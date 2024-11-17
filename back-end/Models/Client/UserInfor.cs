using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Models.Client
{
    public class UserInfor
    {
        [Key]
        public required string Id { get; set; }
        public required string AccountId { get; set; }
        [ForeignKey("AccountId")]
        public required User Account { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public DateTime? DoB { get; set; }
        public string? PhoneNumber { get; set; }

    }
}