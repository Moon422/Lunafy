namespace Lunafy.Core.Domains;

public class Auth : BaseEntity
{
    public int UserId { get; set; }
    public string PasswordHash { get; set; }
}