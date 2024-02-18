namespace Pets.Db.Models
{

    public class PetProfile
    {
        public Guid Id { get; set; }

        public Guid? OwnerId { get; set; }
        public virtual required ApplicationUser Owner { get; set; }
        
        public byte SexId { get; set; }
        public virtual required Sex Sex { get; set; }

        public int BreedId { get; set; }
        public virtual required PetBreed Breed { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool AvailableForBreeding { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool ForSale { get; set; }
        public int Price { get; set; }
        public bool ForAdoption { get; set; }
        public bool Missing { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
