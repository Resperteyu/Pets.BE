using System;

namespace Pets.API.Responses.Dtos
{
    public class ServiceOfferDto
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public PetOwnerInfosDto User { get; set; }

        public ServiceTypeDto ServiceType { get; set; }

        public bool ForCats { get; set; }
        public bool ForDogs { get; set; }
        public bool Active { get; set; }

        public int Rate { get; set; }
        public int PeakRate { get; set; }
        public int HourlyRate { get; set; }
        public int AdditionalPetRate { get; set; }
    }
}
