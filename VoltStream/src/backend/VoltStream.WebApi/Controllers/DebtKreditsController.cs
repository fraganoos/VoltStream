namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.DebtKredits.Commands;
using VoltStream.Application.Features.DebtKredits.Queries;
using VoltStream.WebApi.Models;

public class DebtKreditsController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateDebtKreditCommand command)
     => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(long id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteDebtKreditCommand(id)) });

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateDebtKreditCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllDebtKreditQuery()) });

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(long id)
        => Ok(new Response { Data = await Mediator.Send(new GetDebtKreditByIdQuery(id)) });
}
