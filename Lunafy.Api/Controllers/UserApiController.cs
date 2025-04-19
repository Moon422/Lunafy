using System.Threading.Tasks;
using AutoMapper;
using Lunafy.Api.Models.User;
using Lunafy.Core.Domains;
using Lunafy.Data;
using Lunafy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lunafy.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserApiController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ITransactionManager _transactionManager;
    private readonly IUserService _userService;

    public UserApiController(IMapper mapper,
        ITransactionManager transactionManager,
        IUserService userService)
    {
        _mapper = mapper;
        _transactionManager = transactionManager;
        _userService = userService;
    }

    // [HttpPost("[action]")]
    // public async Task<IActionResult> Login([FromBody] LoginModel model)
    // {

    // }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegistrationModel model)
    {
        var user = (await _userService.GetUserByEmailAsync(model.Email))
            ?? await _userService.GetUserByUseranmeAsync(model.Username);

        if (user is not null)
        {
            return BadRequest("User with email or username already exists.");
        }

        user = _mapper.Map<User>(model);
        try
        {
            await _transactionManager.ExecuteAsync(async () =>
            {
                await _userService.CreateUserAsync(user);
                await _userService.CreatePasswordAsync(user.Id, model.Password);
            });

            var response = _mapper.Map<UserModel>(user);

            return CreatedAtAction(nameof(Details), response);
        }
        catch
        {
            return Problem();
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Details()
    {
        return Ok();
    }
}
