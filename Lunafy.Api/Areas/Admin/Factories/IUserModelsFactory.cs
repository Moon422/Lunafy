using System.Threading.Tasks;
using Lunafy.Api.Areas.Admin.Models.Users;
using Lunafy.Api.Models;
using Lunafy.Core.Domains;

namespace Lunafy.Api.Areas.Admin.Factories;

public interface IUserModelsFactory
{
    Task<UserReadModel> PrepareUserReadModelAsync(UserReadModel model, User user);

    Task<SearchResultModel<UserReadModel>> PrepareUserReadSearchResultAsync(UserSearchCommand searchCommand);
}
