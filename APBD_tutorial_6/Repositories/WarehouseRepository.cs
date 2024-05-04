using System.Data;
using System.Net;
using APBD_tutorial_6.Models;
using Microsoft.Data.SqlClient;

namespace APBD_tutorial_6.Repositories;

public class WarehouseRepository:IWarehouseRepository
{
    private IConfiguration _configuration;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public int AddToWarehouse(Warehouse warehouse)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();
        var validation = new CreateWareHouseValidation(con);
        if (!validation.FullValidation
            (warehouse.IdWarehouse,
                warehouse.IdProduct,
                warehouse.Amount,
                warehouse.CreatedAt))
            return -1;
        CreateWareHouseManipulation manipulation = new CreateWareHouseManipulation(con);
        manipulation.UpdateOrderFulfilled(validation.idOrder,warehouse.CreatedAt);
        return manipulation.InsertProduct_Warehouse(warehouse, validation.idOrder);
    }

    public int AddToWarehouseProcedure(Warehouse warehouse)
    {
        try
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();
            string procedureName = "AddProductToWarehouse";
            SqlCommand command = new SqlCommand(procedureName, con);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse",warehouse.IdWarehouse);
            command.Parameters.AddWithValue("@Amount",warehouse.Amount);
            command.Parameters.AddWithValue("@CreatedAt",warehouse.CreatedAt);
            string strId  = Convert.ToString( command.ExecuteScalar());
            return int.Parse(strId);
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }

    public class CreateWareHouseValidation
    {
        private SqlConnection _connection;
        public int? idOrder;

        public CreateWareHouseValidation(SqlConnection connection)
        {
            _connection = connection;
            idOrder = null;
        }

        public bool FullValidation(int idWarehouse, int idProduct, int amount, DateTime createdAt)
        {
            CheckOrder(idProduct, amount, createdAt);
            return IsWarehouse(idWarehouse) &&
                   IsProduct(idProduct) &&
                   IsValidAmount(amount) &&
                   IsOrder() &&
                   IsNotReleased();
        }

        private bool IsWarehouse(int id)
        {
            using var cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "SELECT 1 FROM Warehouse where IdWarehouse = @idWarehouse";
            cmd.Parameters.AddWithValue("@idWarehouse", id);
            using (var dr = cmd.ExecuteReader())
            {
                return dr.HasRows;
            }
        }

        private bool IsProduct(int id)
        {
            using var cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "SELECT 1 FROM Product where IdProduct = @idProduct";
            cmd.Parameters.AddWithValue("@idProduct", id);
            using (var dr = cmd.ExecuteReader())
            {
                return dr.HasRows;
            }
        }

        private bool IsValidAmount(int amount)
        {
            return amount > 0;
        }

        private void CheckOrder(int idProduct, int amount, DateTime whCreatedAt)
        {
            using var cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "SELECT TOP 1 o.IdOrder  FROM \"Order\" o " +
                              "LEFT JOIN Product_Warehouse pw ON o.IdOrder=pw.IdOrder " +
                              "WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND pw.IdProductWarehouse IS NULL " +
                              "AND o.CreatedAt<@CreatedAt;";
            cmd.Parameters.AddWithValue("@IdProduct", idProduct);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@CreatedAt",whCreatedAt);
            this.idOrder = (int?)cmd.ExecuteScalar();
        }

        private bool IsOrder()
        {
            return this.idOrder != null;
        }

        private bool IsNotReleased()
        {
            using var cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "SELECT 1 FROM Product_Warehouse where IdOrder = @idOrder";
            cmd.Parameters.AddWithValue("@idOrder", this.idOrder);
            using (var dr = cmd.ExecuteReader())
            {
                return !dr.HasRows;
            }
        }
    }

    public class CreateWareHouseManipulation
    {
        private SqlConnection _connection;
        public CreateWareHouseManipulation(SqlConnection connection)
        {
            _connection = connection;
        }

        public void UpdateOrderFulfilled(int? idOrder,DateTime createdAt)
        {
            using var cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = "UPDATE \"Order\" SET FulfilledAt=@CreatedAt  WHERE IdOrder=@IdOrder;";
            cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
            cmd.Parameters.AddWithValue("@IdOrder", idOrder);
            cmd.ExecuteNonQuery();
        }

        public int InsertProduct_Warehouse(Warehouse warehouse, int? idOrder)
        {
            using var cmd = new SqlCommand();
            Decimal price;
            cmd.Connection = _connection;
            cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct=@IdProduct";
            cmd.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            using (var dr = cmd.ExecuteReader())
            {
                dr.Read();
                price = dr.GetDecimal(0);
            }
            using var cmdInsert = new SqlCommand();
            cmdInsert.Connection = _connection;
            cmdInsert.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse,   \n IdProduct, IdOrder, Amount, Price, CreatedAt) OUTPUT inserted.IdProductWarehouse " + 
                                    "VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, @CreatedAt);";
            cmdInsert.Parameters.AddWithValue("@IdWarehouse", warehouse.IdWarehouse);
            cmdInsert.Parameters.AddWithValue("@IdOrder", idOrder);
            cmdInsert.Parameters.AddWithValue("@IdProduct", warehouse.IdProduct);
            cmdInsert.Parameters.AddWithValue("@Amount", warehouse.Amount);
            cmdInsert.Parameters.AddWithValue("@Price", price);
            cmdInsert.Parameters.AddWithValue("@CreatedAt", warehouse.CreatedAt);
            string strId  = Convert.ToString( cmdInsert.ExecuteScalar());
            return int.Parse(strId);
        }
    }
}