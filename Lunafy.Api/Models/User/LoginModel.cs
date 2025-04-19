namespace Lunafy.Api.Models.User;

public record LoginModel : BaseModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
