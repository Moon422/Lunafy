using System.Threading.Tasks;
using Lunafy.Api.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {

    }
}
