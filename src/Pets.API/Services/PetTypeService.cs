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

    public class PetTypeService(PetsDbContext context, IMapper mapper) : IPetTypeService
    {
        public async Task<List<PetTypeDto>> GetAll()
        {
            var petTypes = await context.PetTypes.ToListAsync();
            return mapper.Map<List<PetTypeDto>>(petTypes);
        }
    }
}
