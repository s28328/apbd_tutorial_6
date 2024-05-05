using APBD_tutorial_6.Models;
using APBD_tutorial_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_tutorial_6.Controllers;


[ApiController]
public class WarehouseController:ControllerBase
{
    private IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService service)
    {
        _warehouseService = service;
    }

    [HttpPost]
    [Route("api/AddToWarehouse")]
    public IActionResult AddToWarehouse([FromBody]Warehouse warehouse)
    {
        var affectedCount = _warehouseService.AddToWarehouse(warehouse);
        Console.WriteLine(affectedCount);
        if (affectedCount == -1)
            return new ConflictResult();
        return StatusCode(StatusCodes.Status201Created);
    }
    
    [HttpPost]
    [Route("api/AddToWarehouseProcedure")]
    public IActionResult AddToWarehouseProcedure([FromBody]Warehouse warehouse)
    {
        var affectedCount = _warehouseService.AddToWarehouseProcedure(warehouse);
        Console.WriteLine(affectedCount);
        if (affectedCount == -1)
            return new ConflictResult();
        return StatusCode(StatusCodes.Status201Created);
    }
}