namespace VoltStream.Application.Features.WarehouseItems.Mappers;

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoltStream.Application.Features.Products.DTOs;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Domain.Entities;

public class WarehouseItemMappingProfile :Profile
{
    public WarehouseItemMappingProfile()
    {
        CreateMap<WarehouseItem, WarehouseItemDto>();
        CreateMap<Product, ProductDto>();
    }
}
