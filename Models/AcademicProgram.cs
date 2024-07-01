namespace DistributionListGenerator.Models
{
    public class AcademicProgram
    {
        // Global identifier for the academic program
        public Guid id { get; set; }

        // The full name of the academic program
        public string title { get; set; }

        // The description of the academic program
        public string description { get; set; }

        // The academic program code
        public string code { get; set; }

        // Status of the academic program
        public string status { get; set; }

        // Start date of the academic program
        public DateOnly startDate { get; set; }

        // End date of the academic program
        public DateOnly endDate { get; set; }
    }
}
