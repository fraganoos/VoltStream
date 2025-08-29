namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Products.Commands;
using VoltStream.Application.Features.Products.Queries;
using VoltStream.WebApi.Models;

public class ProductsController : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProductCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(long id)
        => Ok(new Response { Data = await Mediator.Send(new DeleteProductCommand(id)) });

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
        => Ok(new Response { Data = await Mediator.Send(new GetAllProductsQuery()) });

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(long id)
        => Ok(new Response { Data = await Mediator.Send(new GetProductByIdQuery(id)) });
}
