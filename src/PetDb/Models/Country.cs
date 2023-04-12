
namespace PetDb.Models
{

    public class Country
    {
        public string CountryCode { get; set; }

        public string Name { get; set; }

        public string DialCode { get; set; }

        //public virtual ICollection<Profile> Profiles { get; } = new List<Profile>();
    }
}
