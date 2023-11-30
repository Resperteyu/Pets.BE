namespace Pets.API.Responses.Dtos
{
    public class AddressDto
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public CountryDto Country { get; set; }
        public LocationDto Location { get; set; }
    }
}
