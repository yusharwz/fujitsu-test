using System;
using DotNetEnv;

namespace WarehouseManagement
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Env.Load();
            string connectionString = $"Host={Env.GetString("DATABASE_HOST")};Database={Env.GetString("DATABASE_NAME")};Username={Env.GetString("DATABASE_USER")};Password={Env.GetString("DATABASE_PASSWORD")}";

            var warehouseService = new WarehouseService(connectionString);
            var itemService = new ItemService(connectionString);
            var monitoringService = new MonitoringService(connectionString);

            while (true)
            {
                Console.Clear();

                Console.WriteLine("=== Warehouse Management Console App ===");
                Console.WriteLine("1. Add Warehouse");
                Console.WriteLine("2. View Warehouse");
                Console.WriteLine("3. Update Warehouse");
                Console.WriteLine("4. Remove Warehouse");
                Console.WriteLine("5. Add Item");
                Console.WriteLine("6. View Item");
                Console.WriteLine("7. Update Item");
                Console.WriteLine("8. Remove Item");
                Console.WriteLine("9. Get Monitoring List");
                Console.WriteLine("10. Exit");
                Console.WriteLine("----------------------------------------");
                Console.Write("Enter your choice (1-9): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Operations.AddWarehouse(warehouseService);
                        break;
                    case "2":
                        Operations.GetWarehouse(warehouseService);
                        break;
                    case "3":
                        Operations.UpdateWarehouse(warehouseService);
                        break;
                    case "4":
                        Operations.RemoveWarehouse(warehouseService);
                        break;
                    case "5":
                        Operations.AddItem(itemService, warehouseService);
                        break;
                    case "6":
                        Operations.GetItem(itemService);
                        break;
                    case "7":
                        Operations.UpdateItem(itemService, warehouseService);
                        break;
                    case "8":
                        Operations.RemoveItem(itemService);
                        break;
                    case "9":
                        Operations.GetMonitoringList(monitoringService);
                        break;
                    case "10":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 1 to 9.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
