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
        Task<List<ServiceOfferDto>> GetServicesView(Guid userId);
        Task<ServiceOfferDto> GetServiceOfferView(Guid id);
        Task<Guid> Create(CreateServiceOfferRequest model, Guid userId);
        Task Update(UpdateServiceOfferRequest model, ServiceOffer entity);
        Task Delete(ServiceOffer entity);
        Task<List<ServiceOfferSearchResultDto>> Search(SearchServiceOfferParams searchParams);
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

        public async Task<List<ServiceOfferDto>> GetServicesView(Guid userId)
        {
            var serviceOffers = await _context.ServiceOffers.Where(x => x.UserId == userId)
                                            .Where(x => x.Active == true)
                                            .Include(i => i.ServiceType)
                                            .ToListAsync();

            return _mapper.Map<List<ServiceOfferDto>>(serviceOffers);
        }

        public async Task<ServiceOfferDto> GetServiceOfferView(Guid id)
        {
            var serviceOffer = await _context.ServiceOffers.Where(x => x.Id == id)
                                            .Where(x => x.Active == true)
                                            .Include(i => i.User)
                                            .Include(i => i.ServiceType)
                                            .SingleOrDefaultAsync();

            if (serviceOffer == null)
                return null;

            return _mapper.Map<ServiceOfferDto>(serviceOffer);
        }

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

        public async Task<List<ServiceOfferSearchResultDto>> Search(SearchServiceOfferParams searchParams)
        {
            IQueryable<ServiceOffer> services = _context.ServiceOffers
                                            .Include(i => i.User)
                                            .Include(i => i.ServiceType)
                                            .Include(i => i.User.Address)
                                                .ThenInclude(a => a.Location)
                                            .Where(i => i.UserId != searchParams.UserId && i.Active == true);

            if (searchParams.TypeId.HasValue)
            {
                services = services.Where(i => i.ServiceTypeId == searchParams.TypeId);
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

                services = services.Where(i => i.User.Address.Location.GeoLocation.Distance(searchPoint) <= radiusInMeters);
            }

            services = services.OrderByDescending(x => x.CreationDate).Take(SEARCH_MAX_RESULTS);

            return await services.Select(i => new ServiceOfferSearchResultDto
            {
                Id = i.Id,
                ForCats = i.ForCats,
                ForDogs = i.ForDogs,
                Rate = i.Rate,
                PeakRate = i.PeakRate,
                HourlyRate = i.HourlyRate,
                AdditionalPetRate = i.AdditionalPetRate,
                Description = i.Description,
                DistanceFromSearchLocation = (searchPoint != null) ? SearchHelper.CalculateDistanceFromSearchLocation(i.User.Address.Location.GeoLocation.Distance(searchPoint),
                    searchParams.SearchRadiusType.Value) : null,
                User = new PetOwnerInfosDto
                {
                    Id = i.User.Id,
                    UserName = i.User.UserName

                    // TODO: Add owner rough address
                },
                ServiceType = new ServiceTypeDto
                {
                    Id = i.ServiceType.Id,
                    Title = i.ServiceType.Title
                }
            }).ToListAsync();
        }

    }
}
