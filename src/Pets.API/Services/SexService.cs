using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface ISexService
    {
        Task<List<SexDto>> GetAll(); 
    }

    public class SexService(PetsDbContext context, IMapper mapper) : ISexService
    {
        public async Task<List<SexDto>> GetAll()
        {
            var sexes = await context.Sexes.ToListAsync();
            return mapper.Map<List<SexDto>>(sexes);
        }
    }
}
