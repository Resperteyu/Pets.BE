namespace Pets.Db.Models
{
    public class Address
    {
        public int Id { get; set; }
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string City { get; set; }
        public required string Postcode { get; set; }
        public required string CountryCode { get; set; }
        public virtual Country Country { get; set; } = null!;

        public virtual Location Location { get; set; } = null!;

        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
