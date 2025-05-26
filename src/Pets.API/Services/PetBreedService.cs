using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IPetBreedService
    {
        Task<List<PetBreedDto>> GetAll();
    }

    public class PetBreedService(PetsDbContext context, IMapper mapper) : IPetBreedService
    {
        public async Task<List<PetBreedDto>> GetAll()
        {
            var petBreeds = await context.PetBreeds.ToListAsync();
            return mapper.Map<List<PetBreedDto>>(petBreeds);
        }
    }
}
