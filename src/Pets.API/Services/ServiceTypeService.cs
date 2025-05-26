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

    public class ServiceTypeService(PetsDbContext context, IMapper mapper) : IServiceTypeService
    {
        public async Task<List<ServiceTypeDto>> GetAll()
        {
            var serviceTypes = await context.ServiceTypes.ToListAsync();
            return mapper.Map<List<ServiceTypeDto>>(serviceTypes);
        }
    }
}
