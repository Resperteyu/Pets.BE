
namespace PetDb.Models
{

    public class PetBreed
    {
        public int Id { get; set; }

        public byte TypeId { get; set; }

        public string Title { get; set; }

        //public virtual ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();

        //public virtual PetType Type { get; set; }
    }
}
