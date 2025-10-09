namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Products.Commands;
using VoltStream.Application.Features.Products.DTOs;
using VoltStream.Application.Features.Products.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class ProductsController
    : CrudController<ProductDto,
    GetAllProductsQuery,
    GetProductByIdQuery,
    CreateProductCommand,
    UpdateProductCommand,
    DeleteProductCommand>
{
    [HttpGet("categoryId={Id:long}")]
    public async Task<IActionResult> GetAllProductByCategoryId(long Id)
    => Ok(new Response { Data = await Mediator.Send(new GetAllProductsByCategoryIdQuery(Id)) });
}
