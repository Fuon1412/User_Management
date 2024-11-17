using back_end.Models.Client;

namespace back_end.Models.Project
{
    public class Project
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public required User Customer { get; set; }
        public required User Leader { get; set; }
    }
}