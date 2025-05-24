using System;

namespace Pets.API.Responses.Dtos
{
    public class LitterDto
    {
        public Guid Id { get; set; }

        public PetBreedDto Breed { get; set; }

        public PetOwnerInfosDto Owner { get; set; }

        public PetProfileDto MotherPetProfile { get; set; }

        public PetProfileDto FatherPetProfile { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int Size { get; set; }
        public bool Available { get; set; }
    }
}
