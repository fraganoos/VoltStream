namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Customers.Commands;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.Customers.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class CustomersController
    : CrudController<CustomerDto,
        GetAllCustomersQuery,
        GetCustomerByIdQuery,
        CreateCustomerCommand,
        UpdateCustomerCommand,
        DeleteCostumerCommand>
{
    [HttpGet("filter")]
    public async Task<IActionResult> GetFiltered(GetAllFilteringCustomersQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}