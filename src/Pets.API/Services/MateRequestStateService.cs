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

    public class MateRequestStateService : IMateRequestStateService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public MateRequestStateService(PetsDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MateRequestStateDto>> GetAll()
        {
            var mateRequestStates = await _context.MateRequestStates.ToListAsync();
            return _mapper.Map<List<MateRequestStateDto>>(mateRequestStates);
        }
    }
}
