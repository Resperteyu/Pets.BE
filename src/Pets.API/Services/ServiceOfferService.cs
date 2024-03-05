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
using Pets.API.Requests.Search;
using Pets.API.Responses.Dtos.Search;
using Pets.API.Helpers;
using Pets.API.Requests.ServiceOffer;

namespace Pets.API.Services
{
    public interface IServiceOfferService
    {
        Task<ServiceOffer> GetEntityById(Guid id);
        Task<ServiceOfferDto> Get(Guid id);
        Task<List<ServiceOfferDto>> GetByOwnerId(Guid userId);
        //Task<List<PetProfileDto>> GetPetsView(Guid userId);
        Task<Guid> Create(CreateServiceOfferRequest model, Guid userId);
        Task Update(UpdateServiceOfferRequest model, ServiceOffer entity);
        Task Delete(ServiceOffer entity);
        //Task<List<PetProfileDto>> GetMates(PetProfile entity, Guid userId);
        //Task<List<PetSearchResultDto>> Search(SearchParams searchParams);
    }

    public class ServiceOfferService : IServiceOfferService
    {
        private const int SEARCH_MAX_RESULTS = 50;

        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public ServiceOfferService(PetsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<ServiceOffer> GetEntityById(Guid id)
        {
            return await _context.ServiceOffers.Where(x => x.Id == id)
                                            .SingleOrDefaultAsync();
        }
        
        public async Task<ServiceOfferDto> Get(Guid id)
        {
            var serviceOffer = await _context.ServiceOffers.Where(x => x.Id == id)
                                            .Include(i => i.ServiceType)
                                            .SingleOrDefaultAsync();

            if (serviceOffer == null)
                return null;

            return _mapper.Map<ServiceOfferDto>(serviceOffer);
        }        

        public async Task<List<ServiceOfferDto>> GetByOwnerId(Guid userId)
        {
            var serviceOffers = await _context.ServiceOffers.Where(x => x.UserId == userId)
                                            .Include(i => i.ServiceType)
                                            .ToListAsync();

            return _mapper.Map<List<ServiceOfferDto>>(serviceOffers);
        }

        /*
        public async Task<List<PetProfileDto>> GetPetsView(Guid userId)
        {
            var petProfiles = await _context.PetProfiles.Where(x => x.OwnerId == userId)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .OrderByDescending(x => x.CreationDate)
                                            .ToListAsync();

            return _mapper.Map<List<PetProfileDto>>(petProfiles);
        }
        */

        public async Task<Guid> Create(CreateServiceOfferRequest request, Guid userId)
        {
            var serviceOfferEntity = _mapper.Map<ServiceOffer>(request);
            serviceOfferEntity.UserId = userId;
            serviceOfferEntity.CreationDate = DateTime.UtcNow;

            var serviceOffer = await _context.ServiceOffers.AddAsync(serviceOfferEntity);
            await _context.SaveChangesAsync();

            return serviceOffer.Entity.Id;
        }

        public async Task Update(UpdateServiceOfferRequest request, ServiceOffer entity)
        {
            entity.Description = request.Description;
            entity.ForCats = request.ForCats;
            entity.ForDogs = request.ForDogs;
            entity.Active = request.Active;
            entity.Rate = request.Rate;
            entity.PeakRate = request.PeakRate;
            entity.AdditionalPetRate = request.AdditionalPetRate;
            entity.HourlyRate = request.HourlyRate;

            _context.ServiceOffers.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ServiceOffer entity)
        {
            _context.ServiceOffers.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /*
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

            petProfiles = petProfiles.Take(SEARCH_MAX_RESULTS).OrderByDescending(x => x.CreationDate);

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
                DistanceFromSearchLocation = (searchPoint != null) ? SearchHelper.CalculateDistanceFromSearchLocation(i.Owner.Address.Location.GeoLocation.Distance(searchPoint),
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

        public async Task<List<PetProfileDto>> GetMates(PetProfile entity, Guid userId)
        {
            //TO DO: introduce mate preferences associated to specific pet
            var petProfiles = await _context.PetProfiles.Where(x => x.OwnerId == userId)
                                            .Where(x => x.SexId != entity.SexId)
                                            .Where(x => x.AvailableForBreeding == true)
                                            .Where(x => x.Breed.TypeId == entity.Breed.TypeId)
                                            .Include(i => i.Breed)
                                            .Include(i => i.Sex)
                                            .ToListAsync();

            return _mapper.Map<List<PetProfileDto>>(petProfiles);
        }
        */
    }
}
