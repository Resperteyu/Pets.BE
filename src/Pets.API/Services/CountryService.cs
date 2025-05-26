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

    public class CountryService(PetsDbContext context, IMapper mapper) : ICountryService
    {
        public async Task<List<CountryDto>> GetAll()
        {
            var countries = await context.Countries.ToListAsync();
            return mapper.Map<List<CountryDto>>(countries);
        }
    }
}
