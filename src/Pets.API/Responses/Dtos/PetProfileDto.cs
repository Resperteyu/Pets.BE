using System;

namespace Pets.API.Responses.Dtos
{
    public class PetProfileDto
    {
        public Guid Id { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool AvailableForBreeding { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public PetOwnerInfosDto Owner { get; set; }

        public SexDto Sex { get; set; }

        public PetBreedDto Breed { get; set; }
    }
}
