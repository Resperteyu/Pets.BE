using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IPetTypeService
    {
        Task<List<PetTypeDto>> GetAll();
    }

    public class PetTypeService : IPetTypeService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public PetTypeService(PetsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PetTypeDto>> GetAll()
        {
            var petTypes = await _context.PetTypes.ToListAsync();
            return _mapper.Map<List<PetTypeDto>>(petTypes);
        }
    }
}
