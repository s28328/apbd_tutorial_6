using APBD_tutorial_6.Models;

namespace APBD_tutorial_6.Repositories;

public interface IWarehouseRepository
{
    public Task<int> AddToWarehouse(Warehouse warehouse);
    public Task<int> AddToWarehouseProcedure(Warehouse warehouse);
}