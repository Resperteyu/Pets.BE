
namespace Pets.Db.Models
{
    public class MateRequestState
    {
        public byte Id { get; set; }

        public string Title { get; set; }

        public ICollection<MateRequest> MateRequests { get; } = new List<MateRequest>();
    }
}
