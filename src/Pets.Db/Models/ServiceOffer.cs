namespace Pets.Db.Models
{

    public class ServiceOffer
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual required ApplicationUser User { get; set; }
        
        public byte ServiceTypeId { get; set; }
        public virtual required ServiceType ServiceType { get; set; }        

        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public bool ForCats { get; set; }
        public bool ForDogs { get; set; }
        public bool Active { get; set; }

        public int Rate { get; set; }
        public int PeakRate { get; set; }
        public int HourlyRate { get; set; }
        public int AdditionalPetRate { get; set; }
    }
}
