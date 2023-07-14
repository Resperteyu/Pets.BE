using Pets.API.Requests;
using Pets.API.Responses.Dtos;
using Pets.Db.Models;

namespace Pets.API.Helpers
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, PetOwnerInfosDto>();

            CreateMap<Sex, SexDto>();

            CreateMap<PetType, PetTypeDto>();

            CreateMap<PetBreed, PetBreedDto>();

            CreateMap<Country, CountryDto>();

            CreateMap<PetProfile, PetProfileDto>();

            CreateMap<CreatePetRequest, PetProfile>();
        }        
    }
}