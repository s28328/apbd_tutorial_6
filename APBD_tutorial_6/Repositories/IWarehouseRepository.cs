using APBD_tutorial_6.Models;

namespace APBD_tutorial_6.Repositories;

public interface IWarehouseRepository
{
    public int AddToWarehouse(Warehouse warehouse);
    public int AddToWarehouseProcedure(Warehouse warehouse);
}