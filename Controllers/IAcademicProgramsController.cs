using DistributionListGenerator.Models;
namespace DistributionListGenerator.Controllers
{
    public interface IAcademicProgramsController
    {
        List<AcademicProgram> GetPrograms();
    }
}
