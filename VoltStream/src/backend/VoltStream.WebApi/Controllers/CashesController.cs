namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Cash.Commands;
using VoltStream.Application.Features.Cashes.Commands;
using VoltStream.Application.Features.Cashes.Queries;
using VoltStream.WebApi.Models;

public class CashesController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCashCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(long id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteCashCommand(id)) });

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCashCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllCashesQuery()) });

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(long id)
        => Ok(new Response { Data = await Mediator.Send(new GetCashByIdQuery(id)) });

}
