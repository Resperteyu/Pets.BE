
namespace PetDb.Models
{

    public class PetProfile
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public byte SexId { get; set; }

        public int BreedId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool AvailableForBreeding { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //public virtual PetBreed Breed { get; set; }

        //public virtual Profile Owner { get; set; }

        //public virtual Sex Sex { get; set; }
    }
}
