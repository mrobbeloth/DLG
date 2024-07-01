using DistributionListGenerator.Models;

namespace DistributionListGenerator.Controllers
{
    public interface IStudentAcademicPeriodsController
    {
        public List<StudentAcademicPeriod> GetStudentsForAcademicPeriod(Guid acdemicPeriod);
    }
}
