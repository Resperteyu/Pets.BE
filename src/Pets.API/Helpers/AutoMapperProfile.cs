using Pets.API.Authentication.Service.Models;
using Pets.API.Requests;
using Pets.API.Responses.Dtos;
using Pets.Db.Models;

namespace Pets.API.Helpers
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account, AccountResponse>();

            CreateMap<Account, AuthenticateResponse>();

            CreateMap<RegisterRequest, Account>();

            CreateMap<CreateRequest, Account>();

            CreateMap<UpdateRequest, Account>()
              .ForAllMembers(x => x.Condition(
                  (src, dest, prop) =>
                  {
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                  }
              ));

            CreateMap<Sex, SexDto>();

            CreateMap<PetType, PetTypeDto>();

            CreateMap<PetBreed, PetBreedDto>();

            CreateMap<Country, CountryDto>();

            CreateMap<PetProfile, PetProfileDto>();

            CreateMap<Profile, PetOwnerInfosDto>();

            CreateMap<CreatePetRequest, PetProfile>();
        }        
    }
}