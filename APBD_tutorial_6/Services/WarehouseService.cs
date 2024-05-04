using APBD_tutorial_6.Models;
using APBD_tutorial_6.Repositories;

namespace APBD_tutorial_6.Services;

public class WarehouseService:IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    public int AddToWarehouse(Warehouse warehouse)
    {
        return _warehouseRepository.AddToWarehouse(warehouse);
    }

    public int AddToWarehouseProcedure(Warehouse warehouse)
    {
        return _warehouseRepository.AddToWarehouseProcedure(warehouse);
    }
}