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

    public class SexService : ISexService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public SexService(PetsDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SexDto>> GetAll()
        {
            var sexes = await _context.Sexes.ToListAsync();
            return _mapper.Map<List<SexDto>>(sexes);
        }
    }
}
