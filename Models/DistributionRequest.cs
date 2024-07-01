using System.ComponentModel.DataAnnotations;

namespace DistributionListGenerator.Models
{
    public class DistributionRequest
    {
        public DistributionRequest()
        {
            this.department = "";
        }   

        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
        [Display(Name = "Department: ")]
        // required annotation is for form validation, required keyword is for obj construction
        public required string department { get; set; }

    }
}
