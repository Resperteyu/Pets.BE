namespace Pets.API.Responses.Dtos
{
    public class UserProfileDto
    {
        // TODO Both Username and Email are on the claims so really we can remove them from here
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public AddressDto Address { get; set; }
        public string AboutMe { get; set; }
        public string ProfilePhotoUrl { get; set; }
    }
}
