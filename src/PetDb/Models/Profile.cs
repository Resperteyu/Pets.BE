
namespace PetDb.Models
{
    public class Profile
    {
        public Guid Id { get; set; }

        public string CountryCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailVerified { get; set; }

        public bool PhoneVerified { get; set; }

        public int? LocationId { get; set; }

        //public virtual Country CountryCodeNavigation { get; set; }

        //public virtual Location Location { get; set; }

        //public virtual ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();
    }
}
