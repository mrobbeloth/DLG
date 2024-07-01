using DistributionListGenerator.Models;
using DistributionListGenerator.Utility;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Xml.Linq;

namespace DistributionListGenerator.Controllers
{
    public class PersonController(IConfiguration configuration) : IPersonController
    {
        private readonly IConfiguration _configuration = configuration;
        public async Task<string> RetrievePersonData(Guid id, string url, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                HttpResponseMessage response = await client.GetAsync(url + id);
                response.EnsureSuccessStatusCode(); // Throw exception for non-success codes

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        public Person GetPersonData(Guid id)
        {
            Person p = new Person();
            string token = Task.Run(() => EthosUtility.GetAuthorizationToken(_configuration.GetSection("Ethos_API")["auth_url"], _configuration.GetSection("Ethos_API")["key"])).Result;
            string response = Task.Run(() => RetrievePersonData(id, _configuration.GetSection("Ethos_API")["ethos_base_url"] + configuration.GetSection("Ethos_API")["persons_endpoint"], token)).Result;


            // Deserialize the JSON string into a dynamic object
            dynamic jsonDynamic = JsonSerializer.Deserialize<dynamic>(response);

            // Get ID (should be the same)
            JsonElement JsonPerson;
            if (jsonDynamic is JsonElement)
            {
                JsonPerson = (JsonElement)jsonDynamic;
            }
            else
            {
                Console.WriteLine("Error: jsonDynamic is not a JsonElement");
                return p;
            }
            
            p.id = Guid.Parse(JsonPerson.GetProperty("id").GetString());

            // Retrieve the legal name
            JsonElement names = JsonPerson.GetProperty("names");
            if (names.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement name in names.EnumerateArray())
                {
                    JsonElement type = name.GetProperty("type");
                    if (type.GetProperty("category").GetString().Equals("legal"))
                    {                  
                        string fullName;
                        if (name.TryGetProperty("fullName", out JsonElement fullNameElement))
                        {
                            fullName = fullNameElement.GetString();
                        }
                        else
                        {
                            fullName = "";
                        }
                        p.name = fullName;
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: names is not an array");
            }

            // Retrieve the email address
            if (JsonPerson.TryGetProperty("emails", out JsonElement emails))
            {
                if (emails.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement email in emails.EnumerateArray())
                    {
                        if (email.TryGetProperty("preference", out JsonElement type))
                        {
                            if (type.GetString().Equals("primary"))
                            {
                                p.email = email.GetProperty("address").GetString();
                                break;
                            }
                        }
                        else if (email.TryGetProperty("emailType", out JsonElement type2))
                        {
                            p.email = email.GetProperty("address").GetString();
                        }

                    }
                }
                else
                {
                    Console.WriteLine("Error: emails is not an array");
                }
            }

            // Retrieve the roles
            JsonElement roles = JsonPerson.GetProperty("roles");
            if (roles.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement role in roles.EnumerateArray())
                {
                    p.roles.Add(role.GetProperty("role").GetString());
                }
            }
            else
            {
                Console.WriteLine("Error: roles is not an array");
            }


            //Retrieve the colleague info
            JsonElement credentials = JsonPerson.GetProperty("credentials");
            if (credentials.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement credential in credentials.EnumerateArray())
                {
                    JsonElement type = credential.GetProperty("type");
                    if (type.GetString().Equals("colleaguePersonId"))
                    {
                        p.colleagueId = credential.GetProperty("value").GetString();
                    }
                    else if(type.GetString().Equals("colleagueUserName"))  {
                        p.colleagueUsername = credential.GetProperty("value").GetString();
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: credentials is not an array");
            }

            return p;
         }
    }
    
}
