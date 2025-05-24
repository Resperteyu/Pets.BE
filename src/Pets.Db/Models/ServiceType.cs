
namespace Pets.Db.Models
{
    public class ServiceType
    {
        public byte Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public ICollection<ServiceOffer> ServiceOffers { get; } = new List<ServiceOffer>();
    }
}
