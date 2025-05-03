using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models.Home;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IHomeModelsFactory
{
    Task<TotalUsersStatModel> PrepareTotalUsersStatModelAsync();
}
