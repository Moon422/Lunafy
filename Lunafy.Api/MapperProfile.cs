using AutoMapper;
using Lunafy.Api.Models.User;
using Lunafy.Core.Domains;

namespace Lunafy.Api;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<RegistrationModel, User>();
        CreateMap<User, UserModel>();
    }
}