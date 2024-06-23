using Dapper;
using Npgsql;

namespace WarehouseManagement
{
    public class Gudang
    {
        public int KodeGudang { get; set; }
        public string NamaGudang { get; set; } = string.Empty;
    }


    public class Barang
    {
        public int KodeBarang { get; set; }
        public string NamaBarang { get; set; } = string.Empty;
        public decimal HargaBarang { get; set; }
        public int JumlahBarang { get; set; }
        public DateTime TanggalKadaluarsa { get; set; }
        public int KodeGudang { get; set; }
    }


    public class WarehouseRepo
    {
        private readonly string _connectionString;

        public WarehouseRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddWarehouse(Gudang warehouse)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "INSERT INTO Gudang (NamaGudang) VALUES (@NamaGudang) RETURNING KodeGudang";
                        warehouse.KodeGudang = connection.ExecuteScalar<int>(query, warehouse, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding warehouse: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<Gudang> GetAllWarehouses()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Gudang ORDER BY KodeGudang ASC";
                return connection.Query<Gudang>(query);
            }
        }

        public Gudang GetWarehouse(int warehouseCode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Gudang WHERE KodeGudang = @KodeGudang";
                return connection.QuerySingleOrDefault<Gudang>(query, new { KodeGudang = warehouseCode });
            }
        }

        public void UpdateWarehouse(Gudang warehouse)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var currentWarehouse = GetWarehouse(warehouse.KodeGudang);
                        if (currentWarehouse == null)
                        {
                            Console.WriteLine($"Warehouse with code {warehouse.KodeGudang} not found in database.");
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(warehouse.NamaGudang))
                        {
                            currentWarehouse.NamaGudang = warehouse.NamaGudang;
                        }

                        var query = "UPDATE Gudang SET NamaGudang = @NamaGudang WHERE KodeGudang = @KodeGudang";
                        connection.Execute(query, currentWarehouse, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating warehouse: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void RemoveWarehouse(int warehouseCode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var itemExistsQuery = "SELECT COUNT(*) FROM Barang WHERE KodeGudang = @KodeGudang";
                        var itemCount = connection.ExecuteScalar<int>(itemExistsQuery, new { KodeGudang = warehouseCode });

                        if (itemCount > 0)
                        {
                            Console.WriteLine("Tidak dapat menghapus gudang. Masih ada barang terkait dengan gudang ini.");
                        }
                        else
                        {
                            var query = "DELETE FROM Gudang WHERE KodeGudang = @KodeGudang";
                            connection.Execute(query, new { KodeGudang = warehouseCode }, transaction: transaction);
                            transaction.Commit();
                            Console.WriteLine("Gudang berhasil dihapus.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error removing warehouse: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }


    public class ItemRepo
    {
        private readonly string _connectionString;

        public ItemRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddItem(Barang item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"
                            INSERT INTO Barang (NamaBarang, HargaBarang, JumlahBarang, TanggalKadaluarsa, KodeGudang) 
                            VALUES (@NamaBarang, @HargaBarang, @JumlahBarang, @TanggalKadaluarsa, @KodeGudang) 
                            RETURNING KodeBarang";

                        item.KodeBarang = connection.ExecuteScalar<int>(query, item, transaction: transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding item: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public Barang GetItem(int itemCode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Barang WHERE KodeBarang = @ItemCode";
                return connection.QuerySingleOrDefault<Barang>(query, new { ItemCode = itemCode });
            }
        }

        public void UpdateItem(Barang item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "UPDATE Barang SET NamaBarang = @NamaBarang, HargaBarang = @HargaBarang, JumlahBarang = @JumlahBarang, TanggalKadaluarsa = @TanggalKadaluarsa, KodeGudang = @KodeGudang WHERE KodeBarang = @KodeBarang";
                        connection.Execute(query, item, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating item: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void RemoveItem(int itemCode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "DELETE FROM Barang WHERE KodeBarang = @KodeBarang";
                        connection.Execute(query, new { KodeBarang = itemCode }, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error removing item: {ex.Message}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<Gudang> GetWarehouseDropdown()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT KodeGudang, NamaGudang FROM Gudang";
                return connection.Query<Gudang>(query).ToList();
            }
        }
    }


    public class MonitoringRepo
    {
        private readonly string _connectionString;

        public MonitoringRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Barang> GetMonitoringList(string warehouseName, DateTime expiredDate)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"
                    SELECT b.*, g.NamaGudang AS WarehouseName 
                    FROM Barang b
                    JOIN Gudang g ON b.KodeGudang = g.KodeGudang
                    WHERE (@WarehouseName IS NULL OR g.NamaGudang LIKE '%' || @WarehouseName || '%')
                    AND (@ExpiredDate IS NULL OR b.TanggalKadaluarsa = @ExpiredDate)";
                return connection.Query<Barang, Gudang, Barang>(
                    query,
                    (item, warehouse) => { item.KodeGudang = warehouse.KodeGudang; return item; },
                    new { WarehouseName = warehouseName, ExpiredDate = expiredDate },
                    splitOn: "KodeGudang"
                ).ToList();
            }
        }
    }
}
