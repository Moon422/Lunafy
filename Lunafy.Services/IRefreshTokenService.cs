using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
    Task CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
}
