namespace Pets.Db.Models
{
    public class UserProfileInfo
    {
        public int Id { get; set; }
        public string? AboutMe { get; set; }
        public string? ProfilePhotoUrl { get; set; }

        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}