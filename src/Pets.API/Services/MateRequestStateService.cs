using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IMateRequestStateService
    {
        Task<List<MateRequestStateDto>> GetAll(); 
    }

    public class MateRequestStateService(PetsDbContext context, IMapper mapper) : IMateRequestStateService
    {
        public async Task<List<MateRequestStateDto>> GetAll()
        {
            var mateRequestStates = await context.MateRequestStates.ToListAsync();
            return mapper.Map<List<MateRequestStateDto>>(mateRequestStates);
        }
    }
}
