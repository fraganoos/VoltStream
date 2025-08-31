namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.Application.Features.Sales.Queries;
using VoltStream.WebApi.Models;


public class SalesController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSaleCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("{Id:long}")]
    public async Task<IActionResult> Delete(long Id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteSaleCommand(Id)) });

    [HttpGet("{Id:long}")]
    public async Task<IActionResult> Get(long Id)
        => Ok(new Response { Data = await Mediator.Send(new GetSaleByIdQuery(Id)) });

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllSalesQuery()) });
}
