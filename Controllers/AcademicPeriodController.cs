using DistributionListGenerator.Models;
using DistributionListGenerator.Utility;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DistributionListGenerator.Controllers
{
    public class AcademicPeriodController(IConfiguration configuration) : IAcademicPeriodsController
    {
        private List<AdvisingPeriod> advisingPeriods;
        private readonly IConfiguration _configuration = configuration;

        private async Task<string> RetrieveAcademicPeriods(string url, string token)
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

        public List<AdvisingPeriod> GetAcademicPeriods()
        {
            string token = Task.Run(() => EthosUtility.GetAuthorizationToken(_configuration.GetSection("Ethos_API")["auth_url"], _configuration.GetSection("Ethos_API")["key"])).Result;
            string response = Task.Run(() => RetrieveAcademicPeriods(_configuration.GetSection("Ethos_API")["ethos_base_url"]+configuration.GetSection("Ethos_API")["academic_periods_endpoint"], token)).Result;

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
            List<AdvisingPeriod> allAdvisingPeriods = new List<AdvisingPeriod>();
            foreach (JsonObject jsonAP in jsonArray)
            {
                AdvisingPeriod ap = new AdvisingPeriod();
                ap.advisingPeriodId = jsonAP.ContainsKey("id") ? Guid.Parse((string)jsonAP["id"]) : Guid.Empty;
                ap.advisingCode = jsonAP.ContainsKey("code") ? (string)jsonAP["code"] : "";
                ap.advisingDescription = jsonAP.ContainsKey("title") ? (string)jsonAP["title"] : "";
                ap.advisingStartDate = jsonAP.ContainsKey("startOn") ? DateOnly.Parse((string)jsonAP["startOn"].ToString().Substring(0, 10)) : DateOnly.MinValue;
                ap.advisingEndDate = jsonAP.ContainsKey("endOn") ? DateOnly.Parse((string)jsonAP["endOn"].ToString().Substring(0, 10)) : DateOnly.MaxValue;
                ap.registrationStatus = jsonAP.ContainsKey("registration") ? (string)jsonAP["registration"] : "";

                allAdvisingPeriods.Add(ap);
            }
            advisingPeriods = allAdvisingPeriods;
            
            return advisingPeriods;
        }
    }
}
