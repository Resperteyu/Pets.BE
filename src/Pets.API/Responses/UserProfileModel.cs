using Pets.Db.Models;

namespace Pets.API.Responses
{
    public class UserProfileModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string CountryName { get; set; }
        public Location Location { get; set; }
    }
}
