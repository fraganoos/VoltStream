namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.DebtKredits.Commands;
using VoltStream.Application.Features.DebtKredits.Queries;
using VoltStream.WebApi.Models;

public class DebtKreditsController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDebtKreditCommand command)
     => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPut]
    public async Task<IActionResult> Update(UpdateDebtKreditCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("{Id:long}")]
    public async Task<IActionResult> Delete(long Id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteDebtKreditCommand(Id)) });

    [HttpGet("{Id:long}")]
    public async Task<IActionResult> GetById(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetDebtKreditByIdQuery(Id)) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllDebtKreditQuery()) });
}
