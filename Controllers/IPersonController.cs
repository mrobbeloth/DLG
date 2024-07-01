using DistributionListGenerator.Models;

namespace DistributionListGenerator.Controllers
{
    public interface IPersonController
    {
        public Person GetPersonData(Guid id);
    }
}
