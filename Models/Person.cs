namespace DistributionListGenerator.Models
{
    public class Person
    {
        // A global identifier for the person
        public Guid id { get; set; }

        // Name of the person title + first + last + suffix/pedigree
        public string name { get; set; }

        // Email address of the person
        public string email { get; set; }

        // Roles of the person
        public List<String> roles { get; set; } = new List<string>();

        // Colleague ID of the person
        public string colleagueId { get; set; }

        // Colleague Username of the person
        public string colleagueUsername { get; set; }

        public override string ToString()
        {
            string personDescription =  name + " (" + email + ")" + "\n";
            foreach (string role in roles)
            {
                personDescription += role + "\n";
            }
            personDescription += "Colleague ID: " + colleagueId + "\n";
            personDescription += "Colleague Username: " + colleagueUsername + "\n";
            return personDescription;
        }   
    }
}
