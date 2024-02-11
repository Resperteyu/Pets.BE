using NetTopologySuite;
using NetTopologySuite.Geometries;
using Pets.API.Requests;
using Pets.API.Requests.Litter;
using Pets.API.Requests.MateRequest;
using Pets.API.Responses.Dtos;
using Pets.Db.Models;
using Location = Pets.Db.Models.Location;

namespace Pets.API.Helpers
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, PetOwnerInfosDto>();

            CreateMap<Sex, SexDto>();

            CreateMap<MateRequestState, MateRequestStateDto>();

            CreateMap<PetType, PetTypeDto>();

            CreateMap<PetBreed, PetBreedDto>();

            CreateMap<Country, CountryDto>()
                .ReverseMap();

            CreateMap<Location, LocationDto>();

            CreateMap<LocationDto, Location>()
                .ReverseMap();

            CreateMap<PetProfile, PetProfileDto>();

            CreateMap<CreatePetRequest, PetProfile>();

            CreateMap<CreateMateRequestRequest, MateRequest>();

            CreateMap<MateRequest, MateRequestDto>();

            CreateMap<Litter, LitterDto>();

            CreateMap<CreateLitterRequest, Litter>();

            CreateMap<ApplicationUser, UserProfileDto>()
                .ReverseMap();

            CreateMap<AddressDto, Address>()
                .ForMember(dest => dest.Country, opt => opt.MapFrom<CountryResolver>())
                .ReverseMap();
        }        
    }
}