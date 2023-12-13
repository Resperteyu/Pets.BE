using Microsoft.Identity.Client;
namespace Pets.API.Responses.Dtos
{
    public class UserProfileDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public AddressDto Address { get; set; }
    }
}
