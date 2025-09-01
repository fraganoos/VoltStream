namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Supplies.Commands;
using VoltStream.Application.Features.Supplies.Queries;
using VoltStream.WebApi.Models;

public class SuppliesController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSupplyCommand command)
    => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSupplyCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("{Id:long}")]
    public async Task<IActionResult> Delete(long Id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteSupplyCommand(Id)) });

    [HttpGet("{Id:long}")]
    public async Task<IActionResult> GetById(long Id)
        => base.Ok(new Response { Data = await Mediator.Send(new GetSupplyByIdQuery(Id)) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllSuppliesQuery()) });
}
