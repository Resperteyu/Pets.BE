using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetAll(); 
    }

    public class CountryService : ICountryService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public CountryService(PetsDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CountryDto>> GetAll()
        {
            var countries = await _context.Countries.ToListAsync();
            return _mapper.Map<List<CountryDto>>(countries);
        }
    }
}
