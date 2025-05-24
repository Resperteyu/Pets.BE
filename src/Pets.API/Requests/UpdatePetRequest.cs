using System;

namespace Pets.API.Requests
{
    public class UpdatePetRequest
    {
        public Guid Id { get; set; }

        public byte SexId { get; set; }

        public int BreedId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool AvailableForBreeding { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool ForSale { get; set; }

        public int Price { get; set; }

        public bool ForAdoption { get; set; }

        public bool Missing { get; set; }
        public bool Private { get; set; }
    }
}
