using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Npgsql;

namespace WarehouseManagement
{
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

        public IEnumerable<Barang> GetAllItems()
        {
            var items = new List<Barang>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT b.KodeBarang, b.NamaBarang, b.HargaBarang, b.JumlahBarang, b.TanggalKadaluarsa, b.KodeGudang, g.NamaGudang
                    FROM Barang b
                    INNER JOIN Gudang g ON b.KodeGudang = g.KodeGudang
                    ORDER BY g.KodeGudang ASC, b.KodeBarang ASC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new Barang
                            {
                                KodeBarang = reader.GetInt32(0),
                                NamaBarang = reader.GetString(1),
                                HargaBarang = reader.GetDecimal(2),
                                JumlahBarang = reader.GetInt32(3),
                                TanggalKadaluarsa = reader.GetDateTime(4),
                                KodeGudang = reader.GetInt32(5),
                                NamaGudang = reader.GetString(6)
                            };
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public Barang GetItem(int itemCode)
        {
            Barang item = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT b.KodeBarang, b.NamaBarang, b.HargaBarang, b.JumlahBarang, b.TanggalKadaluarsa, b.KodeGudang, g.NamaGudang
                    FROM Barang b
                    INNER JOIN Gudang g ON b.KodeGudang = g.KodeGudang
                    WHERE b.KodeBarang = @ItemCode";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("ItemCode", itemCode);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = new Barang
                            {
                                KodeBarang = reader.GetInt32(0),
                                NamaBarang = reader.GetString(1),
                                HargaBarang = reader.GetDecimal(2),
                                JumlahBarang = reader.GetInt32(3),
                                TanggalKadaluarsa = reader.GetDateTime(4),
                                KodeGudang = reader.GetInt32(5),
                                NamaGudang = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return item;
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

        public IEnumerable<MonitoringItem> GetMonitoringList(string warehouseName, DateTime referenceDate)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"
                    SELECT b.KodeBarang, b.NamaBarang, b.JumlahBarang, b.TanggalKadaluarsa, 
                        g.KodeGudang, g.NamaGudang 
                    FROM Barang b
                    JOIN Gudang g ON b.KodeGudang = g.KodeGudang
                    WHERE (LOWER(g.NamaGudang) LIKE '%' || LOWER(COALESCE(@WarehouseName, '')) || '%')
                    AND (b.TanggalKadaluarsa <= @ReferenceDate OR b.TanggalKadaluarsa <= (@ReferenceDate::DATE + INTERVAL '14 day'))";

                var parameters = new { WarehouseName = (object)warehouseName ?? DBNull.Value, ReferenceDate = referenceDate };

                return connection.Query<MonitoringItem>(query, parameters).ToList();
            }
        }
    }
}
