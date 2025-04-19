using System;
using System.Threading.Tasks;
using Lunafy.Core.Domains;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data;
using Lunafy.Data.Caching;
using Lunafy.Services.Caching;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Services;

[ScopeDependency(typeof(IRefreshTokenService))]
public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly ICacheManager _cacheManager;

    public RefreshTokenService(IRepository<RefreshToken> refreshTokenRepository,
        ICacheManager cacheManager)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _cacheManager = cacheManager;
    }

    public async Task<RefreshToken?> GetRefreshTokenByTokendAsync(string token)
    {
        if (!Guid.TryParse(token, out var _))
            return null;

        var cacheKey = _cacheManager.PrepareCacheKey(RefreshTokenCacheDefaults.ByTokenCacheKey,
            token);

        return await _cacheManager.GetAsync(cacheKey, async () => await _refreshTokenRepository.Table
            .FirstOrDefaultAsync(x => x.Token.ToLower() == token.ToLower()));
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken, nameof(refreshToken));
        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }

    public async Task CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken, nameof(refreshToken));
        await _refreshTokenRepository.InsertAsync(refreshToken);
    }
}