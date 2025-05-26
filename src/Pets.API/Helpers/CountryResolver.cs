using AutoMapper;
using Pets.API.Responses.Dtos;
using Pets.Db;
using Pets.Db.Models;
using System.Linq;

namespace Pets.API.Helpers
{
    public class CountryResolver(PetsDbContext context) : IValueResolver<AddressDto, Address, Country>
    {
        public Country Resolve(AddressDto source, Address destination, Country destMember, ResolutionContext context1)
        {
            if (source.Country == null)
                return null;

            var existingCountry = context.Countries.FirstOrDefault(c => c.Code == source.Country.Code);

            return existingCountry ?? null; // Throw exception?
        }
    }

}
