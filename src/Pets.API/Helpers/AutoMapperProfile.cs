using System;
using AutoMapper;
using Pets.API.Authentication.Service.Models;
using Pets.API.Models;
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
    }
  }
}