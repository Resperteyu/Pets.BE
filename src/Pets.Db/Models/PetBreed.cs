namespace Pets.Db.Models
{

    public class PetBreed
    {
        public int Id { get; set; }

        public byte TypeId { get; set; }

        public string Title { get; set; }

        public ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();
    }
}
