namespace DistributionListGenerator.Utility
{
    public class EthosUtility
    {
        public static async Task<string> GetAuthorizationToken(string url, string key)
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
    }
}
