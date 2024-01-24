using System;

namespace Pets.API.Responses.Dtos
{
    public class MateRequestDto
    {
        public Guid Id { get; set; }
        public PetProfileDto PetProfile { get; set; }
        public Guid PetOwnerId { get; set; }

        public PetProfileDto PetMateProfile { get; set; }
        public Guid PetMateOwnerId { get; set; }

        public string Description { get; set; }
        public string AmountAgreement { get; set; }
        public string LitterSplitAgreement { get; set; }
        public string BreedingPlaceAgreement { get; set; }
        public DateTime CreationDate { get; set; }
        public string Response { get; set; }
        public string Comment { get; set; }

        public bool IsRequester { get; set; }
        public bool IsReceiver { get; set; }

        public MateRequestStateDto MateRequestState { get; set; }
    }
}