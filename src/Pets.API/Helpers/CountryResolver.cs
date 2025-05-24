using AutoMapper;
using Pets.API.Responses.Dtos;
using Pets.Db;
using Pets.Db.Models;
using System.Linq;

namespace Pets.API.Helpers
{
    public class CountryResolver : IValueResolver<AddressDto, Address, Country>
    {
        private readonly PetsDbContext _context;

        public CountryResolver(PetsDbContext context)
        {
            _context = context;
        }

        public Country Resolve(AddressDto source, Address destination, Country destMember, ResolutionContext context)
        {
            if (source.Country == null)
                return null;

            var existingCountry = _context.Countries.FirstOrDefault(c => c.Code == source.Country.Code);

            return existingCountry ?? null; // Throw exception?
        }
    }

}
