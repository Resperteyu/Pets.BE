using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IServiceTypeService
    {
        Task<List<ServiceTypeDto>> GetAll(); 
    }

    public class ServiceTypeService : IServiceTypeService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public ServiceTypeService(PetsDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ServiceTypeDto>> GetAll()
        {
            var serviceTypes = await _context.ServiceTypes.ToListAsync();
            return _mapper.Map<List<ServiceTypeDto>>(serviceTypes);
        }
    }
}
