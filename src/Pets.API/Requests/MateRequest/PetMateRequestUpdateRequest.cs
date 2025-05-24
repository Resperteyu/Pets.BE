using System;

namespace Pets.API.Requests.MateRequest
{
    public class PetMateRequestUpdateRequest
    {
        public Guid MateRequestId { get; set; }

        public string Description { get; set; }

        public string AmountAgreement { get; set; }

        public string LitterSplitAgreement { get; set; }

        public string BreedingPlaceAgreement { get; set; }
    }
}
