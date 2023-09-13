using Microsoft.AspNetCore.Identity;
using PetDb.Models;

namespace Pets.Db.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public virtual Country? Country { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public virtual Location? Location { get; set; }

        public virtual ICollection<PetProfile> PetProfiles { get; set; } = new List<PetProfile>();

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
    }

}
