using DistributionListGenerator.Models;
using DistributionListGenerator.Utility;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

/* Getting Forbidden 403 with current token */

namespace DistributionListGenerator.Controllers
{    
    public class StudentAcademicProgramsController(IConfiguration configuration) : IStudentAcademicProgramsController
    {
        private readonly IConfiguration _configuration = configuration;
        public async Task<string> RetrievePrograms(string url, string token)
        {
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);


                var enrollmentStatus = new
                {
                    enrollmentStatus = new
                    {
                        status = "active" // Only get active students
                    }
                };
                string json = JsonSerializer.Serialize(enrollmentStatus);

                HttpResponseMessage response = await client.GetAsync(url+json);
                response.EnsureSuccessStatusCode(); // Throw exception for non-success codes

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
        List<StudentAcademicProgram> IStudentAcademicProgramsController.GetPrograms()
        {
            string token = Task.Run(() => EthosUtility.GetAuthorizationToken(_configuration.GetSection("Ethos_API")["auth_url"], _configuration.GetSection("Ethos_API")["key"])).Result;
            string response = Task.Run(() => RetrievePrograms(_configuration.GetSection("Ethos_API")["ethos_base_url"] + configuration.GetSection("Ethos_API")["student_academic_programs_endpoint"] +"?criteria=", token)).Result;

            return new List<StudentAcademicProgram>();
        }


    }
}
