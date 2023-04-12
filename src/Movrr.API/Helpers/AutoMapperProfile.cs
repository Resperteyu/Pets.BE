using System;
using AutoMapper;
using Movrr.API.Authentication.Service.Models;
using Movrr.API.Models;
using PetDb.Models;

namespace Movrr.API.Helpers
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
    }
  }
}