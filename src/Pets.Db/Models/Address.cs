namespace Pets.Db.Models
{
    public class Address
    {
        public int Id { get; set; }
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string City { get; set; }
        public required string Postcode { get; set; }
        public required Country Country { get; set; }

        public required Location Location { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; } = new List<ApplicationUser>();
    }
}
