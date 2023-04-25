using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Requests;
using Pets.API.Responses.Dtos;
using Pets.Db;
using Pets.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IPetProfileService
    {
        Task<PetProfile> GetEntityByPetId(Guid petId);
        Task<PetProfileDto> GetByPetId(Guid petId);
        Task<List<PetProfileDto>> GetByOwnerId(Guid petId);
        Task<Guid> CreatePet(CreatePetRequest model, Guid ownerId);
        Task UpdatePet(UpdatePetRequest model, PetProfile entity);
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

        public async Task<PetProfile> GetEntityByPetId(Guid petId)
        {
            return await _context.PetProfiles.FindAsync(petId);
        }

        public async Task<PetProfileDto> GetByPetId(Guid petId)
        {
            var petProfile = await _context.PetProfiles.Where(x => x.Id == petId)
                                            .Include(i => i.Owner)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .SingleOrDefaultAsync();

            if (petProfile == null)
                return null;

            return _mapper.Map<PetProfileDto>(petProfile);
        }

        public async Task<List<PetProfileDto>> GetByOwnerId(Guid ownerId)
        {
            var petProfiles = await _context.PetProfiles.Where(x => x.OwnerId == ownerId)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .ToListAsync();

            return _mapper.Map<List<PetProfileDto>>(petProfiles);
        }

        public async Task<Guid> CreatePet(CreatePetRequest request, Guid ownerId)
        {
            var pet = _mapper.Map<PetProfile>(request);
            pet.OwnerId = ownerId;

            var petProfile = await _context.PetProfiles.AddAsync(pet);
            await _context.SaveChangesAsync();

            return petProfile.Entity.Id;
        }

        public async Task UpdatePet(UpdatePetRequest request, PetProfile entity)
        {
            entity.SexId = request.SexId;
            entity.BreedId = request.BreedId;
            entity.DateOfBirth = request.DateOfBirth;
            entity.AvailableForBreeding = request.AvailableForBreeding;
            entity.Name = request.Name;
            entity.Description = request.Description;

            _context.PetProfiles.Update(entity);
            await _context.SaveChangesAsync();
        }   
    }
}
