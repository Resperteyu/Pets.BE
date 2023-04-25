using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IPetProfileService
    {
        Task<PetProfileDto> GetByPetId(Guid petId);
        Task<List<PetProfileDto>> GetByOwnerId(Guid petId);
    }

    public class PetProfileService : IPetProfileService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public PetProfileService(PetsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PetProfileDto> GetByPetId(Guid petId)
        {
            var petProfile = await _context.PetProfiles.Where(x => x.Id == petId)
                                            .Include(i => i.Owner)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .SingleOrDefaultAsync();
            if (petProfile == null)
            {
                return null;
            }
            return _mapper.Map<PetProfileDto>(petProfile);
        }

        public async Task<List<PetProfileDto>> GetByOwnerId(Guid ownerId)
        {
            var petProfiles = await _context.PetProfiles.Where(x => x.OwnerId == ownerId)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex).ToListAsync();

            return _mapper.Map<List<PetProfileDto>>(petProfiles);
        }
    }
}
