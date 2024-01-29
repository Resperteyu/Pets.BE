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
using NetTopologySuite.Geometries;
using System.Threading;

namespace Pets.API.Services
{
    public interface IPetProfileService
    {
        Task<PetProfile> GetEntityByPetId(Guid petId);
        Task<PetProfileDto> GetByPetId(Guid petId);
        Task<List<PetProfileDto>> GetByOwnerId(Guid petId);
        Task<Guid> CreatePet(CreatePetRequest model, Guid ownerId);
        Task UpdatePet(UpdatePetRequest model, PetProfile entity);
        Task DeletePet(PetProfile entity);
        Task<List<PetProfileDto>> GetMates(PetProfile entity, Guid userId);
        Task<List<PetSearchResultDto>> Search(SearchParams searchParams);
    }

    public class PetProfileService : IPetProfileService
    {
        private const int SEARCH_MAX_RESULTS = 50;

        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public PetProfileService(PetsDbContext context, IMapper mapper,
            IImageStorageService imageStorageService)
        {
            _context = context;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        public async Task<PetProfile> GetEntityByPetId(Guid petId)
        {
            return await _context.PetProfiles.Where(x => x.Id == petId)
                                            .Include(i => i.Breed)
                                            .SingleOrDefaultAsync();
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
            entity.ForSale = request.ForSale;
            entity.Price = request.Price;
            entity.ForAdoption  = request.ForAdoption;
            entity.Missing = request.Missing;

            _context.PetProfiles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePet(PetProfile entity)
        {
            _context.PetProfiles.Remove(entity);
            await _context.SaveChangesAsync();
            await _imageStorageService.DeleteAllPetImages(entity.Id, CancellationToken.None);
        }

        public async Task<List<PetSearchResultDto>> Search(SearchParams searchParams)
        {
            IQueryable<PetProfile> petProfiles = _context.PetProfiles
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .Include(i => i.Owner)
                                            .Include(i => i.Owner.Address)
                                                .ThenInclude(a => a.Location)
                                            .Where(i => i.OwnerId != searchParams.UserId);

            if(searchParams.AvailableForBreeding.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.AvailableForBreeding == searchParams.AvailableForBreeding);
            }

            if (searchParams.ForSale.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.ForSale == searchParams.ForSale);
            }

            if (searchParams.ForAdoption.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.ForAdoption == searchParams.ForAdoption);
            }

            if (searchParams.Missing.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.Missing == searchParams.Missing);
            }

            if (searchParams.SexId.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.SexId == searchParams.SexId);
            }

            if (searchParams.Age.HasValue && searchParams.Age > 0)
            {
                DateTime targetDateOfBirth = DateTime.Today.AddYears(-searchParams.Age.Value);
                petProfiles = petProfiles.Where(i => i.DateOfBirth <= targetDateOfBirth);
            }

            if (searchParams.TypeId.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.Breed.TypeId == searchParams.TypeId);
            }

            if (searchParams.BreedId.HasValue)
            {
                petProfiles = petProfiles.Where(i => i.BreedId == searchParams.BreedId);
            }

            Point searchPoint = null;
            if (searchParams.SearchRadiusType != SearchRadiusType.Unknown &&
                searchParams.SearchRadius.HasValue && searchParams.Latitude.HasValue &&
                searchParams.Longitude.HasValue)
            {
                double radiusInMeters = searchParams.SearchRadiusType switch
                {
                    SearchRadiusType.Miles => searchParams.SearchRadius.Value * 1609.34, // Convert miles to meters
                    SearchRadiusType.Kilometers => searchParams.SearchRadius.Value * 1000, // Convert kilometers to meters
                };

                var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                searchPoint = geometryFactory.CreatePoint(new Coordinate(searchParams.Longitude.Value, searchParams.Latitude.Value));

                petProfiles = petProfiles.Where(i => i.Owner.Address.Location.GeoLocation.Distance(searchPoint) <= radiusInMeters);
            }

            petProfiles = petProfiles.Take(SEARCH_MAX_RESULTS);

            return await petProfiles.Select(i => new PetSearchResultDto
            {
                Id = i.Id,
                AvailableForBreeding = i.AvailableForBreeding,
                ForSale = i.ForSale,
                ForAdoption = i.ForAdoption,
                Missing = i.Missing,
                Breed = new PetBreedDto
                {
                    Id = i.Breed.Id,
                    Title = i.Breed.Title,
                    TypeId = i.Breed.TypeId
                },
                DateOfBirth = i.DateOfBirth,
                Description = i.Description,
                DistanceFromSearchLocation = (searchPoint != null) ? CalculateDistanceFromSearchLocation(i.Owner.Address.Location.GeoLocation.Distance(searchPoint),
                    searchParams.SearchRadiusType.Value) : null,
                Owner = new PetOwnerInfosDto
                {
                    Id = i.Owner.Id,
                    UserName = i.Owner.UserName
                    
                    // TODO: Add owner rough address
                },
                Name = i.Name,
                Sex = new SexDto
                {
                    Id = i.Sex.Id,
                    Title = i.Sex.Title
                }
            }).ToListAsync();
        }

        private static double CalculateDistanceFromSearchLocation(double distanceInMeters, SearchRadiusType searchRadiusType)
        {
            double distance = searchRadiusType switch
            {
                SearchRadiusType.Miles => Math.Round(distanceInMeters * 0.000621371, 1),
                SearchRadiusType.Kilometers => Math.Round(distanceInMeters * 0.001, 1)
            };

            return distance;
        }

        public async Task<List<PetProfileDto>> GetMates(PetProfile entity, Guid userId)
        {
            //TO DO: introduce mate preferences associated to specific pet
            var petProfiles = await _context.PetProfiles.Where(x => x.OwnerId == userId)
                                            .Where(x => x.SexId != entity.SexId)
                                            .Where(x => x.Breed.TypeId == entity.Breed.TypeId)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .ToListAsync();

            return _mapper.Map<List<PetProfileDto>>(petProfiles);
        }
    }
}
