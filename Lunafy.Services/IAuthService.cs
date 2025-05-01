using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Services;

public interface IAuthService
{
    Task GenerateRefreshToken(User user);
    Task<string> GenerateJwtToken(User user);
}