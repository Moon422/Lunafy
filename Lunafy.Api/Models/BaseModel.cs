namespace Lunafy.Api.Models;

public abstract record BaseModel
{ }

public abstract record BaseEntityModel : BaseModel
{
    public int Id { get; set; }
}