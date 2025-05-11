using Lunafy.Api.Models.Artist;
using Lunafy.Api.Models.User;
using Lunafy.Core.Domains;
using Lunafy.Services;
using Microsoft.AspNetCore.Identity.Data;

namespace Lunafy.Api;

public static class Mapper
{
    public static UserModel ToModel(this User entity)
    {
        return new UserModel
        {
            Id = entity.Id,
            Firstname = entity.Firstname,
            Lastname = entity.Lastname,
            Username = entity.Username,
            Email = entity.Email,
            IsAdmin = entity.IsAdmin,
            IsArtist = entity.IsArtist,
            IsInactive = entity.IsInactive,
            InactiveTill = entity.InactiveTill,
            RequirePasswordReset = entity.RequirePasswordReset,
            LastLogin = entity.LastLogin,
            CreatedOn = entity.CreatedOn,
            ModifiedOn = entity.ModifiedOn,
            Deleted = entity.Deleted,
            DeletedOn = entity.DeletedOn
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

    public static User ToEntity(this RegistrationModel model)
    {
        return new User
        {
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Username = model.Username,
            Email = model.Email,
        };
    }

    public static void UpdateEntity(this UserModel model, User user)
    {
        var entity = model.ToEntity();
        entity.IsInactive = user.IsInactive;
        entity.InactiveTill = user.InactiveTill;
        entity.RequirePasswordReset = user.RequirePasswordReset;
        entity.LastLogin = user.LastLogin;
        entity.ModifiedOn = user.ModifiedOn;
        entity.Deleted = user.Deleted;
        entity.DeletedOn = user.DeletedOn;
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
}