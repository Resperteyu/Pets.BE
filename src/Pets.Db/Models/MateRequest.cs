
namespace Pets.Db.Models
{
    public class MateRequest
    {
        public Guid Id { get; set; }
        public Guid PetProfileId { get; set; }
        public virtual required PetProfile PetProfile { get; set; }
        public Guid PetOwnerId { get; set; }
        public Guid PetMateProfileId { get; set; }
        public virtual required PetProfile PetMateProfile { get; set; }
        public Guid PetMateOwnerId { get; set; }

        public DateTime CreationDate { get; set; }
        public string? Description { get; set; }
        public string? AmountAgreement { get; set; }
        public string? LitterSplitAgreement { get; set; }
        public string? BreedingPlaceAgreement { get; set; }
        public string? Response { get; set; }
        public string? Comment { get; set; }
        public byte MateRequestStateId { get; set; }
        public virtual required MateRequestState MateRequestState { get; set; }
    }
}
