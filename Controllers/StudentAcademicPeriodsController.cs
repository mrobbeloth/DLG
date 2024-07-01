using DistributionListGenerator.Models;
using DistributionListGenerator.Utility;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DistributionListGenerator.Controllers
{
    public class StudentAcademicPeriodsController(IConfiguration configuration) : IStudentAcademicPeriodsController
    {
        private readonly IConfiguration _configuration = configuration;
        private async Task<string> RetrieveStudentsForAcademicPeriod(Guid theAcademicPeriod, string url, string token)
        {
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var academicPeriod = new
                {
                    academicPeriod = new
                    {
                        id = theAcademicPeriod // Replace with your actual ID value
                    }
                };

                string json = JsonSerializer.Serialize(academicPeriod);

                HttpResponseMessage response = await client.GetAsync(url+json);
                response.EnsureSuccessStatusCode(); // Throw exception for non-success codes

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
        
        public List<StudentAcademicPeriod> GetStudentsForAcademicPeriod(Guid acdemicPeriod)
        {
            List<StudentAcademicPeriod> saps = new List<StudentAcademicPeriod>();
            string token = Task.Run(() => EthosUtility.GetAuthorizationToken(_configuration.GetSection("Ethos_API")["auth_url"], _configuration.GetSection("Ethos_API")["key"])).Result;
            string response = Task.Run(() => RetrieveStudentsForAcademicPeriod(acdemicPeriod, _configuration.GetSection("Ethos_API")["ethos_base_url"] + configuration.GetSection("Ethos_API")["student_academic_periods_endpoint"] + "?criteria=", token)).Result;

            // Deserialize the JSON string into a dynamic object
            dynamic jsonObject = JsonSerializer.Deserialize<dynamic>(response);
            JsonArray jsonArray = new JsonArray();
            if (jsonObject.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in jsonObject.EnumerateArray())
                {
                    jsonArray.Add(element);
                }
            }
            else
            {
                throw new InvalidOperationException("jsonObject is not an array");
            }

            foreach (JsonObject jsonAP in jsonArray)
            {
                StudentAcademicPeriod sap = new StudentAcademicPeriod();
                bool v = jsonAP.TryGetPropertyValue("person", out JsonNode studentId);
                sap.studentId = (Guid)studentId["id"];  
                saps.Add(sap);                
            }
            return saps;
        }
    }
}
