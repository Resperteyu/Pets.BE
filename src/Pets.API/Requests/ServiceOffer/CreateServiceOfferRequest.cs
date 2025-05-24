
namespace Pets.API.Requests.ServiceOffer
{
    public class CreateServiceOfferRequest
    {
        public byte ServiceTypeId { get; set; }

        public bool ForCats { get; set; } = false;

        public bool ForDogs { get; set; } = false;

        public bool Active { get; set; } = false;

        public int Rate { get; set; } = 0;

        public int PeakRate { get; set; }

        public int HourlyRate { get; set; }

        public int AdditionalPetRate { get; set; }

        public string Description { get; set; }
    }
}
