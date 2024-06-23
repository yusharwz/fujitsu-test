using System;
using DotNetEnv;

namespace WarehouseManagement
{
   public class Delivery
   {
      public static void Menu(string[] args)
      {
         Env.Load();
         string connectionString = Config.ConnectionString;

         var warehouseService = new WarehouseRepo(connectionString);
         var itemService = new ItemRepo(connectionString);
         var monitoringService = new MonitoringRepo(connectionString);

         while (true)
         {
               Console.Clear();

               Console.WriteLine("=== Aplikasi Konsol Manajemen Gudang ===");
               Console.WriteLine("1. Add Gudang");
               Console.WriteLine("2. Get Gudang");
               Console.WriteLine("3. Update Gudang");
               Console.WriteLine("4. Delete Gudang");
               Console.WriteLine("5. Add Barang");
               Console.WriteLine("6. Get Barang");
               Console.WriteLine("7. Update Barang");
               Console.WriteLine("8. Delete Barang");
               Console.WriteLine("9. List Monitoring");
               Console.WriteLine("10. Keluar");
               Console.WriteLine("----------------------------------------");
               Console.Write("Masukkan pilihan Anda (1-10): ");

               string choice = Console.ReadLine();

               switch (choice)
               {
                  case "1":
                     Usecase.AddWarehouse(warehouseService);
                     break;
                  case "2":
                     Usecase.GetWarehouse(warehouseService);
                     break;
                  case "3":
                     Usecase.UpdateWarehouse(warehouseService);
                     break;
                  case "4":
                     Usecase.RemoveWarehouse(warehouseService);
                     break;
                  case "5":
                     Usecase.AddItem(itemService, warehouseService);
                     break;
                  case "6":
                     Usecase.GetItem(itemService);
                     break;
                  case "7":
                     Usecase.UpdateItem(itemService, warehouseService);
                     break;
                  case "8":
                     Usecase.RemoveItem(itemService);
                     break;
                  case "9":
                     Usecase.GetMonitoringList(monitoringService);
                     break;
                  case "10":
                     Console.WriteLine("Keluar...");
                     return;
                  default:
                     Console.WriteLine("Pilihan tidak valid. Silakan masukkan angka dari 1 sampai 10.");
                     break;
               }

               Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
               Console.ReadKey();
         }
      }
   }
}
