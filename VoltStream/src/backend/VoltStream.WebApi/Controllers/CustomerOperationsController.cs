namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.CustomerOperations.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class CustomerOperationsController : BaseController
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllCustomerOperationsQuery()) });

    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(CustomerOperationFilterQuery query)
      => Ok(new Response { Data = await Mediator.Send(query) });


    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetByCustomerId(long customerId,
        [FromQuery] DateTime? beginDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        return Ok(new Response
        {
            Data = await Mediator.Send(
            new GetCustomerOperationByCustomerIdQuery(customerId, beginDate, endDate))
        });
    }
}
