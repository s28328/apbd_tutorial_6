using APBD_tutorial_6.Models;

namespace APBD_tutorial_6.Services;

public interface IWarehouseService
{
    public Task<int> AddToWarehouse(Warehouse warehouse);
    public Task<int> AddToWarehouseProcedure(Warehouse warehouse);

}