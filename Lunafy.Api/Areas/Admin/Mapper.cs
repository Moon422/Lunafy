using Lunafy.Api.Areas.Admin.Models.Artists;
using Lunafy.Api.Areas.Admin.Models.Users;
using Lunafy.Core.Domains;
using Lunafy.Services;

namespace Lunafy.Api.Areas.Admin;

public static class Mapper
{
    public static UserModel ToModel(this User user)
    {
        return new UserModel
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Username = user.Username,
            Email = user.Email,
            IsAdmin = user.IsAdmin,
            IsArtist = user.IsArtist,
            IsInactive = user.IsInactive,
            InactiveTill = user.InactiveTill,
            RequirePasswordReset = user.RequirePasswordReset,
            LastLogin = user.LastLogin,
            CreatedOn = user.CreatedOn,
            ModifiedOn = user.ModifiedOn,
            Deleted = user.Deleted,
            DeletedOn = user.DeletedOn,
        };
    }

    public static User ToEntity(this UserModel model)
    {
        return new User
        {
            Id = model.Id,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Username = model.Username,
            Email = model.Email,
            IsAdmin = model.IsAdmin,
            IsArtist = model.IsArtist
        };
    }

    public static User UpdateEntity(this UserModel model, User user)
    {
        var entity = model.ToEntity();
        entity.IsInactive = user.IsInactive;
        entity.InactiveTill = user.InactiveTill;
        entity.RequirePasswordReset = user.RequirePasswordReset;
        entity.LastLogin = user.LastLogin;
        entity.CreatedOn = user.CreatedOn;
        entity.ModifiedOn = user.ModifiedOn;
        entity.Deleted = user.Deleted;
        entity.DeletedOn = user.DeletedOn;

        return entity;
    }

    public static ArtistModel ToModel(this Artist artist)
    {
        return new ArtistModel
        {
            Id = artist.Id,
            Firstname = artist.Firstname,
            Lastname = artist.Lastname,
            Biography = artist.Biography,
            MusicBrainzId = artist.MusicBrainzId,
            CreatedOn = artist.CreatedOn,
            ModifiedOn = artist.ModifiedOn,
            Deleted = artist.Deleted,
            DeletedOn = artist.DeletedOn
        };
    }

    public static Artist ToEntity(this ArtistModel model)
    {
        return new Artist
        {
            Id = model.Id,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Biography = model.Biography,
            MusicBrainzId = model.MusicBrainzId,
            CreatedOn = model.CreatedOn,
            ModifiedOn = model.ModifiedOn,
            Deleted = model.Deleted,
            DeletedOn = model.DeletedOn
        };
    }

    public static Artist UpdateEntity(this ArtistModel model, Artist artist)
    {
        var entity = model.ToEntity();
        entity.CreatedOn = artist.CreatedOn;
        entity.ModifiedOn = artist.ModifiedOn;
        entity.Deleted = artist.Deleted;
        entity.DeletedOn = artist.DeletedOn;

        return entity;
    }

    public static FindArtistsCommand ToFindCommand(this ArtistSearchCommand command)
    {
        return new FindArtistsCommand
        {
            Firstname = command.Firstname,
            Lastname = command.Lastname,
            Keyword = command.Keyword,
            PageNumber = command.PageNumber,
            PageSize = command.PageSize
        };
    }

    public static FindUsersCommand ToFindCommand(this UserSearchCommand command)
    {
        return new FindUsersCommand
        {
            Firstname = command.Firstname,
            Lastname = command.Lastname,
            Username = command.Username,
            Email = command.Email,
            Keyword = command.Keyword,
            CreatedOnFromUtc = command.CreatedOnFromUtc,
            CreatedOnTillUtc = command.CreatedOnTillUtc,
            PageNumber = command.PageNumber,
            PageSize = command.PageSize
        };
    }
}