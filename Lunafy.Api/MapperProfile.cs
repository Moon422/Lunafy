using AutoMapper;
using Lunafy.Api.Models.Artist;
using Lunafy.Api.Models.User;
using Lunafy.Core.Domains;
using Lunafy.Services;

namespace Lunafy.Api;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // admin models
        CreateMap<User, Areas.Admin.Models.Users.UserReadModel>();
        CreateMap<Areas.Admin.Models.Users.UserCreateModel, User>();
        CreateMap<Areas.Admin.Models.Users.UserSearchCommand, FindUsersCommand>();

        // public models
        CreateMap<RegistrationModel, User>();
        CreateMap<User, UserModel>();
        CreateMap<Artist, ArtistReadModel>();
        CreateMap<ArtistWriteModel, Artist>()
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.Deleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedOn, opt => opt.Ignore());
        CreateMap<ArtistSearchCommand, FindArtistsCommand>();
    }
}