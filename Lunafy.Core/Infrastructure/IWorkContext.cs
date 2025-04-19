using System.Threading.Tasks;
using Lunafy.Core.Domains;

namespace Lunafy.Core.Infrastructure;

public interface IWorkContext
{
    Task<User?> GetCurrentUserAsync();
}