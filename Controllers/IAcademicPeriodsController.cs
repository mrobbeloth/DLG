using DistributionListGenerator.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace DistributionListGenerator.Controllers
{
    public interface IAcademicPeriodsController
    {
        public List<AdvisingPeriod> GetAcademicPeriods();
    }
}
