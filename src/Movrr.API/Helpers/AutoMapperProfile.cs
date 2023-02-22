using System;
using AutoMapper;
using Movrr.API.Authentication.Service.Entities;
using Movrr.API.Authentication.Service.Models;
using Movrr.API.Models;

namespace Movrr.API.Helpers
{
  public class AutoMapperProfile : Profile
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