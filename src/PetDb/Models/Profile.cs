
namespace PetDb.Models
{
    public class Profile
    {
        public Guid Id { get; set; }

        //start old account
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string VerificationToken { get; set; }
        public DateTime? Verified { get; set; }
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        /*
        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
        */
        //end old account

        public string CountryCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailVerified { get; set; }

        public bool PhoneVerified { get; set; }

        public int? LocationId { get; set; }

        //public virtual Country CountryCodeNavigation { get; set; }

        //public virtual Location Location { get; set; }

        //public virtual ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();
    }
}
