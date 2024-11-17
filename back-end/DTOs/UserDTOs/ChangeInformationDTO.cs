namespace back_end.DTOs.UserDTOs
{
    public class ChangeInformationDTO
    {
        public string? FullName { get; set; } = String.Empty;
        public string? PhoneNumber { get; set; } = String.Empty;
        public DateTime? DateOfBirth { get; set; } 
    }
}