namespace Lunafy.Api.Models.User;

public record LoginResponseModel : BaseModel
{
    public UserModel User { get; set; }
    public string Jwt { get; set; }
}
