using APBD_tutorial_6.Models;

namespace APBD_tutorial_6.Services;

public interface IWarehouseService
{
    public int AddToWarehouse(Warehouse warehouse);
    public int AddToWarehouseProcedure(Warehouse warehouse);

}