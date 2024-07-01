using DistributionListGenerator.Models;
namespace DistributionListGenerator.Controllers
{
    public interface IStudentAcademicProgramsController
    {
        List<StudentAcademicProgram> GetPrograms();
    }
}
