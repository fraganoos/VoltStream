namespace VoltStream.WebApi.Controllers;

using VoltStream.Application.Features.Warehouses.Commands;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Application.Features.Warehouses.Queries;
using VoltStream.WebApi.Controllers.Common;

public class WarehousesController
    : CrudController<WarehouseDTO,
        GetAllWarehouseQuery,
        GetWarehouseByIdQuery,
        CreateWarehouseCommand,
        UpdateWarehouseCommand,
        DeleteWarehouseCommand>;
