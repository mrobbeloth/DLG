namespace DistributionListGenerator.Models
{
    public class AdvisingPeriod
    {
        // A global identifier of an academic period.
        public Guid advisingPeriodId { get; set; }

        //A code that identifies an academic period.
        public string? advisingCode { get; set; }

        // The full name of an academic period
        public string? advisingDescription { get; set; }

        // The start date of an academic period
        public DateOnly advisingStartDate { get; set; }

        // The end date of an academic period
        public DateOnly advisingEndDate { get; set; }

        // Registration status
        public string? registrationStatus { get; set; }
    }
}
