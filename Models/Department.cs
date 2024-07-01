namespace DistributionListGenerator.Models
{
    public class Department
    {
        // All departments use api endpoint /api/departments
        // Active ones use /api/departments/active
        // seems to be from an ancient ver, use academic-programs

        // Unique sytem code for this department
        public string code { get; set; }

        // Department Description
        public string description { get; set; }

        // Department Division
        public string division { get; set; }

        // Department School
        public string school { get; set; }

    }
}
