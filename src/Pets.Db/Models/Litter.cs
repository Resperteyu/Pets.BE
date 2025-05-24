namespace Pets.Db.Models
{

    public class Litter
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }
        public virtual required ApplicationUser Owner { get; set; }
        
        public int BreedId { get; set; }
        public virtual required PetBreed Breed { get; set; }

        public Guid MotherPetId { get; set; }
        public virtual required PetProfile MotherPetProfile { get; set; }

        public Guid FatherPetId { get; set; }
        public virtual required PetProfile FatherPetProfile { get; set; }

        public DateTime CreationDate { get; set; }

        public int Size { get; set; }
        public bool Available { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
