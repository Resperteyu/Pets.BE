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

    public class PetBreedService : IPetBreedService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public PetBreedService(PetsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PetBreedDto>> GetAll()
        {
            var petBreeds = await _context.PetBreeds.ToListAsync();
            return _mapper.Map<List<PetBreedDto>>(petBreeds);
        }
    }
}
