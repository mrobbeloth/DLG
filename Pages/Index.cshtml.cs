using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DistributionListGenerator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using DistributionListGenerator.Controllers;

namespace DistributionListGenerator.Pages
{
    public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
        private readonly IAcademicPeriodsController _academicPeriodsController;
        private readonly IStudentAcademicPeriodsController _studentAcademicPeriodsController;
        private readonly IPersonController _personController;
        private readonly IStudentAcademicProgramsController _studentAcademicProgramsController;
        private readonly IAcademicProgramsController _academicProgramsController;

        [BindProperty]
        public DistributionRequest dr { get; set; }
		public List<AdvisingPeriod> aps { get; set; }

        [BindProperty]
        public string AcademicPeriod { get; set; }
        public List<SelectListItem> Academic_Periods { get; set; }
        public List<StudentAcademicPeriod> studentsInaAP { get; set; }
        public List<Person> people { get; set; } = new List<Person>();

        public List<AcademicProgram> programs { get; set; }

        public List<SelectListItem> AcademicPrograms { get; set; }

        [BindProperty]
        public string ChosenAcademicProgram { get; set; }

        public List <Department> departments { get; set; }

        [BindProperty]
        public List <SelectListItem> depts { get; set; }

        [BindProperty]
        public string ChosenDepartment { get; set; }

		public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
		{
			_logger = logger;
            _personController  = new PersonController(configuration);
            _studentAcademicProgramsController = new StudentAcademicProgramsController(configuration);

            // Will generate 403 w/ current token
            // _studentAcademicProgramsController.GetPrograms();

            _academicPeriodsController = new AcademicPeriodController(configuration);
            aps = _academicPeriodsController.GetAcademicPeriods();

            // Get Academic Periods since founding
            Academic_Periods = new List<SelectListItem>();
            foreach (AdvisingPeriod ap in aps)
            {
                SelectListItem s = new SelectListItem(ap.advisingDescription, ap.advisingPeriodId.ToString());
                Academic_Periods.Add(s);
            }

            // Get Active Academic Programs
            _academicProgramsController = new AcademicProgramsController(configuration);
            programs = _academicProgramsController.GetPrograms();
            programs.Sort((x, y) => x.title.CompareTo(y.title));
            AcademicPrograms = new List<SelectListItem>();
            foreach (AcademicProgram prog in programs)
            {
                SelectListItem s = new SelectListItem(prog.title, prog.id.ToString());
                AcademicPrograms.Add(s);
            }            

            _studentAcademicPeriodsController = new StudentAcademicPeriodsController(configuration);
            studentsInaAP = _studentAcademicPeriodsController.GetStudentsForAcademicPeriod(aps[200].advisingPeriodId);
            foreach (StudentAcademicPeriod sap in studentsInaAP)
            {
                // Console.WriteLine(sap.studentId);+
                Person p = _personController.GetPersonData(sap.studentId);
                // Console.WriteLine(p);
                people.Add(p);
            }
        }

        public void OnGet()
		{           
		}

		public async Task<string> GetAuthorizationToken(string url, string key)
		{
            using (HttpClient client = new HttpClient())
			{
                StringContent content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + key);
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Throw exception for non-success codes

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

		public async Task<string> GetAcademicPeriods(string url, string token)
		{
            using (HttpClient client = new HttpClient())
            {
				
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw exception for non-success codes

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

		public async void OnPost()
		{

            if (ModelState.IsValid)
			{
                Console.WriteLine("Department: " + dr.department);
            }
        }
	}
}
