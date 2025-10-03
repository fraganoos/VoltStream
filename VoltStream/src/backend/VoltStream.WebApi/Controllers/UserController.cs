namespace VoltStream.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Users.Commands;
using VoltStream.Application.Features.Users.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class UserController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });
}
