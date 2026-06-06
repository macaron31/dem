using Microsoft.Data.Sqlite;
using System.Data;

namespace WpfApp.Models
{
    internal class CRUD
    {
        private readonly CreateDataBase database = new CreateDataBase();

        private DataTable LoadTable(SqliteCommand command)
        {
            DataTable table = new DataTable();
            SqliteDataReader reader = command.ExecuteReader();
            table.Load(reader);
            return table;
        }

        // =====================================================
        // SUPPLIERS
        // =====================================================
        public DataTable GetSuppliers()
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            // Пишем явный синоним AS ID большими буквами
            string query = $@"
            SELECT ID AS ID, {Config.Table1Attribute1}, {Config.Table1Attribute2}, {Config.Table1Attribute3}, {Config.Table1Attribute4}
            FROM {Config.Table1Name}";

            SqliteCommand command = new SqliteCommand(query, connection);
            return LoadTable(command);
        }


        public void AddSupplier(string vendorTitle, string email, string phone, bool isActive)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            // Убран лишний слэш перед знаком доллара!
            string query = $@"
    INSERT INTO {Config.Table1Name} ({Config.Table1Attribute1}, {Config.Table1Attribute2}, {Config.Table1Attribute3}, {Config.Table1Attribute4})
    VALUES (@vendorTitle, @email, @phone, @isActive)";

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@vendorTitle", vendorTitle);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);

            command.ExecuteNonQuery();
        }


        public void UpdateSupplier(int id, string vendorTitle, string email, string phone, bool isActive)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            // Проверь, чтобы в строке ниже было строго @isActive (с буквой 'e' на конце!)
            string query = $@"
    UPDATE {Config.Table1Name}
    SET {Config.Table1Attribute1} = @vendorTitle, 
        {Config.Table1Attribute2} = @email, 
        {Config.Table1Attribute3} = @phone, 
        {Config.Table1Attribute4} = @isActive
    WHERE ID = @id";

            SqliteCommand command = new SqliteCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@vendorTitle", vendorTitle);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@isActive", isActive ? 1 : 0); // И здесь тоже @isActive

            command.ExecuteNonQuery();
        }


        public void DeleteSupplier(int id)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"DELETE FROM {Config.Table1Name} WHERE ID = @id";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        // =====================================================
        // STOCK ITEMS
        // =====================================================
        public DataTable GetStockItems()
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            // Пишем явный синоним AS ID
            string query = $@"
            SELECT ID AS ID, {Config.Table2Attribute1}, {Config.Table2Attribute2}, {Config.Table2Attribute3}
            FROM {Config.Table2Name}";

            SqliteCommand command = new SqliteCommand(query, connection);
            return LoadTable(command);
        }

        public void AddStockItem(string title, double cost, string details)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"
            INSERT INTO {Config.Table2Name} ({Config.Table2Attribute1}, {Config.Table2Attribute2}, {Config.Table2Attribute3})
            VALUES (@title, @cost, @details)";

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@cost", cost);
            command.Parameters.AddWithValue("@details", details);

            command.ExecuteNonQuery();
        }

        public void UpdateStockItem(int id, string title, double cost, string details)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"
            UPDATE {Config.Table2Name}
            SET {Config.Table2Attribute1} = @title, {Config.Table2Attribute2} = @cost, {Config.Table2Attribute3} = @details
            WHERE ID = @id";

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@cost", cost);
            command.Parameters.AddWithValue("@details", details);

            command.ExecuteNonQuery();
        }

        public void DeleteStockItem(int id)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"DELETE FROM {Config.Table2Name} WHERE ID = @id";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        // =====================================================
        // PROCUREMENTS
        // =====================================================
        public DataTable GetProcurements()
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            // Пишем явный синоним o.ID AS ID
            string query = $@"
            SELECT o.ID AS ID, p.{Config.Table1Attribute1}, o.{Config.Table3Attribute2}, o.{Config.Table3Attribute3}, o.{Config.Table3Attribute4}
            FROM {Config.Table3Name} o
            JOIN {Config.Table1Name} p ON o.{Config.Table3Attribute1} = p.ID";

            SqliteCommand command = new SqliteCommand(query, connection);
            return LoadTable(command);
        }

        public void AddProcurement(int supplierId, string stage)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"
            INSERT INTO {Config.Table3Name} ({Config.Table3Attribute1}, {Config.Table3Attribute3})
            VALUES (@supplierId, @stage)";

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@supplierId", supplierId);
            command.Parameters.AddWithValue("@stage", stage);
            command.ExecuteNonQuery();
        }

        public void UpdateProcurement(int id, string stage)
        {
            var conn = database.GetConnection();
            conn.Open();

            var cmd = new SqliteCommand($@"
            UPDATE {Config.Table3Name}
            SET {Config.Table3Attribute3} = @stage, {Config.Table3Attribute4} = CURRENT_TIMESTAMP
            WHERE ID = @id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@stage", stage);
            cmd.ExecuteNonQuery();
        }

        public void DeleteProcurement(int id)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"DELETE FROM {Config.Table3Name} WHERE ID = @id";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        // =====================================================
        // SUPPLY LINES
        // =====================================================
        public DataTable GetSupplyLines(int procurementId)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"
            SELECT oi.ID, p.{Config.Table2Attribute1}, oi.{Config.Table4Attribute3}, oi.{Config.Table4Attribute4}
            FROM {Config.Table4Name} oi
            JOIN {Config.Table2Name} p ON oi.{Config.Table4Attribute2} = p.ID
            WHERE oi.{Config.Table4Attribute1} = @procurementId";

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@procurementId", procurementId);
            return LoadTable(command);
        }

        public void AddSupplyLine(int procurementId, int itemId, int count, double price)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"
            INSERT INTO {Config.Table4Name} ({Config.Table4Attribute1}, {Config.Table4Attribute2}, {Config.Table4Attribute3}, {Config.Table4Attribute4})
            VALUES (@procurementId, @itemId, @count, @price)";

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@procurementId", procurementId);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@count", count);
            command.Parameters.AddWithValue("@price", price);

            command.ExecuteNonQuery();
        }
        public void DeleteSupplyLine(int id)
        {
            SqliteConnection connection = database.GetConnection();
            connection.Open();

            string query = $@"DELETE FROM {Config.Table4Name} WHERE ID = @id";
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }


        public void UpdateSupplyLine(int id, int count, double price)
        {
             var conn = database.GetConnection();
            conn.Open();

             var cmd = new SqliteCommand($@"
        UPDATE {Config.Table4Name}
        SET {Config.Table4Attribute3} = @count,
            {Config.Table4Attribute4} = @price
        WHERE ID = @id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@count", count);
            cmd.Parameters.AddWithValue("@price", price);

            cmd.ExecuteNonQuery();
        }
    }
}

