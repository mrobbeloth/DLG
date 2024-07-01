using DistributionListGenerator.Models;
using DistributionListGenerator.Utility;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace DistributionListGenerator.Controllers
{
    public class AcademicProgramsController(IConfiguration configuration) : IAcademicProgramsController
    {
        private readonly IConfiguration _configuration = configuration;
        private async Task<string> RetrieveAcademicPrograms(string url, string token)
        {
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);


                var filter = new
                {
                    recruitmentProgram = "active"
                };

                string json = JsonSerializer.Serialize(filter);

                // verified that url+json is correct, but inactive programs are still being returned
                HttpResponseMessage response = await client.GetAsync(url+json);
                response.EnsureSuccessStatusCode(); // Throw exception for non-success codes

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        public List<AcademicProgram> GetPrograms()
        {
            string token = Task.Run(() => EthosUtility.GetAuthorizationToken(_configuration.GetSection("Ethos_API")["auth_url"], _configuration.GetSection("Ethos_API")["key"])).Result;
            string response = Task.Run(() => RetrieveAcademicPrograms(_configuration.GetSection("Ethos_API")["ethos_base_url"] + configuration.GetSection("Ethos_API")["academic-programs"] + "?recruitmentProgram=", token)).Result;
            
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
            List<AcademicProgram> allAcdemicPrograms = new List<AcademicProgram>();
            foreach (JsonObject jsonAP in jsonArray)
            {
                AcademicProgram ap = new AcademicProgram();
                ap.id = jsonAP.ContainsKey("id") ? Guid.Parse((string)jsonAP["id"]) : Guid.Empty;
                ap.code = jsonAP.ContainsKey("code") ? (string)jsonAP["code"] : "";
                ap.title = jsonAP.ContainsKey("title") ? (string)jsonAP["title"] : "";
                ap.description = jsonAP.ContainsKey("description") ? (string)jsonAP["description"] : "";
                if (jsonAP.ContainsKey("status") && ((string)jsonAP["status"]).Equals("active"))
                {
                    ap.status = "active";
                }
                else
                {
                    continue;
                }
                ap.startDate = jsonAP.ContainsKey("startOn") ? DateOnly.Parse((string)jsonAP["startOn"].ToString().Substring(0, 10)) : DateOnly.MinValue;
                ap.endDate = jsonAP.ContainsKey("endOn") ? DateOnly.Parse((string)jsonAP["endOn"].ToString().Substring(0, 10)) : DateOnly.MaxValue;
                allAcdemicPrograms.Add(ap);
            }

            return allAcdemicPrograms;
        }
    }
}
