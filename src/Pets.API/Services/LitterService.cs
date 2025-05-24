using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses.Dtos;
using Pets.Db;
using Pets.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Pets.API.Requests.Litter;
using NetTopologySuite.Geometries;
using Pets.API.Requests.Search;
using Pets.API.Requests;
using Pets.API.Responses.Dtos.Search;
using Pets.API.Helpers;

namespace Pets.API.Services
{
    public interface ILitterService
    {
        Task<Litter> GetEntityById(Guid id);
        Task<LitterDto> GetById(Guid id);
        Task<List<LitterDto>> GetLittersView(Guid id);
        Task<Guid> CreateLitter(CreateLitterRequest model, Guid ownerId);
        Task UpdateLitter(UpdateLitterRequest model, Litter entity);
        Task DeleteLitter(Litter entity);
        Task<List<LitterSearchResultDto>> Search(SearchLitterParams searchParams);
    }

    public class LitterService : ILitterService
    {
        private const int SEARCH_MAX_RESULTS = 50;

        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public LitterService(PetsDbContext context, IMapper mapper,
            IImageStorageService imageStorageService)
        {
            _context = context;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        public async Task<Litter> GetEntityById(Guid id)
        {
            return await _context.Litters.Where(x => x.Id == id)
                                            .Include(i => i.Breed)
                                            .SingleOrDefaultAsync();
        }

        public async Task<LitterDto> GetById(Guid id)
        {
            var litter = await _context.Litters.Where(x => x.Id == id)
                                            .Include(i => i.Owner)
                                            .Include(i => i.Breed)
                                            .Include(i => i.FatherPetProfile)
                                            .Include(i => i.MotherPetProfile)
                                            .SingleOrDefaultAsync();

            if (litter == null)
                return null;

            return _mapper.Map<LitterDto>(litter);
        }

        public async Task<List<LitterDto>> GetLittersView(Guid userId)
        {
            var litters = await _context.Litters.Where(x => x.OwnerId == userId)                                            
                                            .Include(i => i.Breed)
                                            .ToListAsync();

            return _mapper.Map<List<LitterDto>>(litters);
        }

        public async Task<Guid> CreateLitter(CreateLitterRequest request, Guid ownerId)
        {
            var litter = _mapper.Map<Litter>(request);
            litter.OwnerId = ownerId;
            litter.CreationDate = DateTime.UtcNow;

            var litterInfo = await _context.Litters.AddAsync(litter);
            await _context.SaveChangesAsync();

            return litterInfo.Entity.Id;
        }

        public async Task UpdateLitter(UpdateLitterRequest request, Litter entity)
        {
            entity.Available = request.Available;
            entity.Title = request.Title;
            entity.Description = request.Description;

            _context.Litters.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLitter(Litter entity)
        {
            _context.Litters.Remove(entity);
            await _context.SaveChangesAsync();
            await _imageStorageService.DeleteAllPetImages(entity.Id, CancellationToken.None);
        }

        public async Task<List<LitterSearchResultDto>> Search(SearchLitterParams searchParams)
        {
            IQueryable<Litter> litters = _context.Litters
                                            .Include(i => i.Owner)
                                            .Include(i => i.Breed)
                                            .Include(i => i.FatherPetProfile)
                                            .Include(i => i.MotherPetProfile)
                                            .Include(i => i.Owner.Address)
                                                .ThenInclude(a => a.Location)
                                            .Where(i => i.OwnerId != searchParams.UserId && i.Available == true);

            if (searchParams.TypeId.HasValue)
            {
                litters = litters.Where(i => i.Breed.TypeId == searchParams.TypeId);
            }

            if (searchParams.BreedId.HasValue)
            {
                litters = litters.Where(i => i.BreedId == searchParams.BreedId);
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

                litters = litters.Where(i => i.Owner.Address.Location.GeoLocation.Distance(searchPoint) <= radiusInMeters);
            }

            litters = litters.OrderByDescending(x => x.CreationDate).Take(SEARCH_MAX_RESULTS);

            return await litters.Select(i => new LitterSearchResultDto
            {
                Id = i.Id,
                Breed = new PetBreedDto
                {
                    Id = i.Breed.Id,
                    Title = i.Breed.Title,
                    TypeId = i.Breed.TypeId
                },
                Description = i.Description,
                DistanceFromSearchLocation = (searchPoint != null) ? SearchHelper.CalculateDistanceFromSearchLocation(i.Owner.Address.Location.GeoLocation.Distance(searchPoint),
                    searchParams.SearchRadiusType.Value) : null,
                Owner = new PetOwnerInfosDto
                {
                    Id = i.Owner.Id,
                    UserName = i.Owner.UserName

                    // TODO: Add owner rough address
                },
                MotherPetProfile = new PetProfileDto
                {
                    Id = i.MotherPetProfile.Id,
                    Name = i.MotherPetProfile.Name
                },
                FatherPetProfile = new PetProfileDto
                {
                    Id = i.FatherPetProfile.Id,
                    Name = i.FatherPetProfile.Name
                },
                Size = i.Size,
                Title = i.Title
            }).ToListAsync();
        }
    }
}
