using System;
using System.Linq;

namespace WarehouseManagement
{
   
      public class Usecase
      {
         public static void GetWarehouse(WarehouseRepo warehouseService)
         {
            Console.Write("Enter Warehouse Code (leave empty to display all): ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
               var warehouses = warehouseService.GetAllWarehouses();
               foreach (var warehouse in warehouses)
               {
                     Console.WriteLine($"Warehouse Code: {warehouse.KodeGudang}, Warehouse Name: {warehouse.NamaGudang}");
               }
            }
            else if (int.TryParse(input, out int warehouseCode))
            {
               var warehouse = warehouseService.GetWarehouse(warehouseCode);
               if (warehouse != null)
               {
                     Console.WriteLine($"Warehouse Code: {warehouse.KodeGudang}, Warehouse Name: {warehouse.NamaGudang}");
               }
               else
               {
                     Console.WriteLine("Warehouse not found.");
               }
            }
            else
            {
               Console.WriteLine("Invalid Warehouse Code.");
            }
         }

         public static void AddWarehouse(WarehouseRepo warehouseService)
         {
            Console.WriteLine("\n=== Add Warehouse ===");

            string warehouseName;
            do
            {
               Console.Write("Enter Warehouse Name: ");
               warehouseName = Console.ReadLine();
               if (string.IsNullOrWhiteSpace(warehouseName))
               {
                  Console.WriteLine("Warehouse name cannot be empty. Please enter a valid name.");
               }
            } while (string.IsNullOrWhiteSpace(warehouseName));

            var newWarehouse = new Gudang { NamaGudang = warehouseName };
            warehouseService.AddWarehouse(newWarehouse);

            var warehouse = warehouseService.GetWarehouse(newWarehouse.KodeGudang);
            if (warehouse != null)
            {
               Console.WriteLine($"Added Warehouse: WarehouseCode: {warehouse.KodeGudang}, WarehouseName: {warehouse.NamaGudang}");
            }
            else
            {
               Console.WriteLine("Warehouse not found or could not be retrieved.");
            }
         }

         public static void UpdateWarehouse(WarehouseRepo warehouseService)
         {
            Console.WriteLine("\n=== Update Warehouse ===");
            Console.Write("Enter Warehouse Code to update: ");
            int warehouseCode;
            while (!int.TryParse(Console.ReadLine(), out warehouseCode))
            {
               Console.WriteLine("Invalid warehouse code. Please enter a valid integer.");
               Console.Write("Enter Warehouse Code to update: ");
            }

            var warehouse = warehouseService.GetWarehouse(warehouseCode);
            if (warehouse == null)
            {
               Console.WriteLine($"Warehouse with code {warehouseCode} not found.");
               return;
            }

            string newWarehouseName;
            do
            {
               Console.Write("Enter new Warehouse Name (leave empty to keep current): ");
               newWarehouseName = Console.ReadLine();
               if (string.IsNullOrWhiteSpace(newWarehouseName))
               {
                     newWarehouseName = warehouse.NamaGudang;
               }
            } while (string.IsNullOrWhiteSpace(newWarehouseName));

            warehouse.NamaGudang = newWarehouseName;
            warehouseService.UpdateWarehouse(warehouse);

            Console.WriteLine("Warehouse updated successfully.");
         }

         public static void RemoveWarehouse(WarehouseRepo warehouseService)
         {
            Console.WriteLine("\n=== Remove Warehouse ===");
            int warehouseCode;

            do
            {
               Console.Write("Enter Warehouse Code to remove: ");
               string input = Console.ReadLine();

               if (!int.TryParse(input, out warehouseCode))
               {
                     Console.WriteLine("Invalid warehouse code. Please enter a valid integer.");
               }
               else
               {
                  var warehouse = warehouseService.GetWarehouse(warehouseCode);
                  if (warehouse == null)
                  {
                     Console.WriteLine($"Warehouse with code {warehouseCode} not found.");
                     return;
                  }
               }
            } while (warehouseCode <= 0);

            warehouseService.RemoveWarehouse(warehouseCode);
            Console.WriteLine($"Warehouse removed successfully.");
         }

         public static void AddItem(ItemRepo itemService, WarehouseRepo warehouseService)
         {
            Console.WriteLine("\n=== Add Item ===");

            string itemName;
            decimal itemPrice;
            int itemQuantity;
            DateTime expiredDate;
            int warehouseCode;

            do
            {
               Console.Write("Enter Item Name: ");
               itemName = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(itemName));

            do
            {
               Console.Write("Enter Item Price: ");
               string inputPrice = Console.ReadLine();
               if (!decimal.TryParse(inputPrice, out itemPrice))
               {
                  Console.WriteLine("Invalid item price. Please enter a valid decimal number.");
               }
            } while (itemPrice <= 0);

            do
            {
               Console.Write("Enter Item Quantity: ");
               string inputQuantity = Console.ReadLine();
               if (!int.TryParse(inputQuantity, out itemQuantity))
               {
                  Console.WriteLine("Invalid item quantity. Please enter a valid integer number.");
               }
            } while (itemQuantity <= 0);

            do
            {
               Console.Write("Enter Expired Date (YYYY-MM-DD): ");
               string inputExpiredDate = Console.ReadLine();
               if (!DateTime.TryParse(inputExpiredDate, out expiredDate))
               {
                  Console.WriteLine("Invalid date format. Please enter a valid date in YYYY-MM-DD format.");
               }
            } while (expiredDate == DateTime.MinValue);

            bool isValidWarehouse = false;
            do
            {
               Console.Write("Enter Warehouse Code: ");
               string inputWarehouseCode = Console.ReadLine();
               if (!int.TryParse(inputWarehouseCode, out warehouseCode))
               {
                  Console.WriteLine("Invalid warehouse code. Please enter a valid integer number.");
                  continue;
               }

               var warehouse = warehouseService.GetWarehouse(warehouseCode);
               if (warehouse == null)
               {
                  Console.WriteLine($"Warehouse with code {warehouseCode} does not exist.");
               }
               else
               {
                  isValidWarehouse = true;
               }
            } while (!isValidWarehouse);

            var newItem = new Barang
            {
               NamaBarang = itemName,
               HargaBarang = itemPrice,
               JumlahBarang = itemQuantity,
               TanggalKadaluarsa = expiredDate,
               KodeGudang = warehouseCode
            };

            itemService.AddItem(newItem);
            Console.WriteLine($"Item added successfully.");
         }

         public static void GetItem(ItemRepo itemService)
         {
            Console.Write("Enter Item Code (leave empty to display all items): ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
               var items = itemService.GetAllItems();
               foreach (var item in items)
               {
                  Console.WriteLine($"Item Code: {item.KodeBarang}");
                  Console.WriteLine($"Item Name: {item.NamaBarang}");
                  Console.WriteLine($"Item Price: {item.HargaBarang}");
                  Console.WriteLine($"Item Quantity: {item.JumlahBarang}");
                  Console.WriteLine($"Expired Date: {item.TanggalKadaluarsa.ToString("yyyy-MM-dd")}");
                  Console.WriteLine($"Warehouse Code: {item.KodeGudang}");
                  Console.WriteLine("----------------------------------------");
               }
            }
            else if (int.TryParse(input, out int itemCode))
            {
               var item = itemService.GetItem(itemCode);
               if (item != null)
               {
                  Console.WriteLine($"Item Code: {item.KodeBarang}");
                  Console.WriteLine($"Item Name: {item.NamaBarang}");
                  Console.WriteLine($"Item Price: {item.HargaBarang}");
                  Console.WriteLine($"Item Quantity: {item.JumlahBarang}");
                  Console.WriteLine($"Expired Date: {item.TanggalKadaluarsa.ToString("yyyy-MM-dd")}");
                  Console.WriteLine($"Warehouse Code: {item.KodeGudang}");
               }
               else
               {
                     Console.WriteLine("Item not found.");
               }
            }
            else
            {
               Console.WriteLine("Invalid Item Code.");
            }
         }



         public static void UpdateItem(ItemRepo itemService, WarehouseRepo warehouseService)
         {
            Console.WriteLine("\n=== Update Item ===");
            Console.Write("Enter Item Code to update: ");
            int itemCode;
            while (!int.TryParse(Console.ReadLine(), out itemCode))
            {
               Console.WriteLine("Invalid input. Please enter a valid Item Code.");
               Console.Write("Enter Item Code to update: ");
            }

            var item = itemService.GetItem(itemCode);
            if (item == null)
            {
               Console.WriteLine($"Item with code {itemCode} not found.");
               return;
            }

            Console.Write("Enter new Item Name (leave empty to keep current value): ");
            string newItemName = Console.ReadLine();

            decimal newItemPrice = 0;
            Console.Write("Enter new Item Price (leave empty to keep current value): ");
            string inputPrice = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputPrice))
            {
               while (!decimal.TryParse(inputPrice, out newItemPrice) || newItemPrice <= 0)
               {
                     Console.WriteLine("Invalid input. Item Price must be a positive number.");
                     Console.Write("Enter new Item Price (leave empty to keep current value): ");
                     inputPrice = Console.ReadLine();
               }
            }
            else
            {
               newItemPrice = item.HargaBarang; 
            }

            int newItemQuantity = 0;
            Console.Write("Enter new Item Quantity (leave empty to keep current value): ");
            string inputQuantity = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputQuantity))
            {
               while (!int.TryParse(inputQuantity, out newItemQuantity) || newItemQuantity <= 0)
               {
                     Console.WriteLine("Invalid input. Item Quantity must be a positive integer.");
                     Console.Write("Enter new Item Quantity (leave empty to keep current value): ");
                     inputQuantity = Console.ReadLine();
               }
            }
            else
            {
               newItemQuantity = item.JumlahBarang; 
            }

            DateTime? newExpiredDate = null;
            Console.Write("Enter new Expired Date (YYYY-MM-DD) (leave empty to keep current value): ");
            string inputExpiredDate = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputExpiredDate))
            {
               while (!DateTime.TryParse(inputExpiredDate, out DateTime tempDate))
               {
                     Console.WriteLine("Invalid input. Please enter a valid date in the format YYYY-MM-DD.");
                     Console.Write("Enter new Expired Date (YYYY-MM-DD) (leave empty to keep current value): ");
                     inputExpiredDate = Console.ReadLine();
               }
               newExpiredDate = DateTime.Parse(inputExpiredDate);
            }
            else
            {
               newExpiredDate = item.TanggalKadaluarsa;
            }

            int newWarehouseCode = 0;
            Console.Write("Enter new Warehouse Code (leave empty to keep current value): ");
            string inputWarehouseCode = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputWarehouseCode))
            {
               while (!int.TryParse(inputWarehouseCode, out newWarehouseCode) || newWarehouseCode <= 0)
               {
                     Console.WriteLine("Invalid input. Warehouse Code must be a positive integer.");
                     Console.Write("Enter new Warehouse Code (leave empty to keep current value): ");
                     inputWarehouseCode = Console.ReadLine();
               }
            }
            else
            {
               newWarehouseCode = item.KodeGudang;
            }

            if (!string.IsNullOrWhiteSpace(newItemName))
            {
               item.NamaBarang = newItemName;
            }

            if (newItemPrice > 0)
            {
               item.HargaBarang = newItemPrice;
            }

            if (newItemQuantity > 0)
            {
               item.JumlahBarang = newItemQuantity;
            }

            if (newExpiredDate != null)
            {
               item.TanggalKadaluarsa = newExpiredDate.Value;
            }

            if (newWarehouseCode > 0)
            {
               var warehouse = warehouseService.GetWarehouse(newWarehouseCode);
               if (warehouse == null)
               {
                     Console.WriteLine($"Warehouse with code {newWarehouseCode} not found. Item Warehouse Code remains unchanged.");
               }
               else
               {
                     item.KodeGudang = newWarehouseCode;
               }
            }

            itemService.UpdateItem(item);
            Console.WriteLine($"Item updated successfully.");
         }

         public static void RemoveItem(ItemRepo itemService)
         {
            Console.WriteLine("\n=== Remove Item ===");
            Console.Write("Enter Item Code to remove: ");
            int itemCode = Convert.ToInt32(Console.ReadLine());

            var item = itemService.GetItem(itemCode);
            if (item == null)
            {
               Console.WriteLine($"Item with code {itemCode} not found.");
               return;
            }

            itemService.RemoveItem(itemCode);
            Console.WriteLine($"Item removed successfully.");
         }

         public static void GetMonitoringList(MonitoringRepo monitoringService)
         {
            Console.WriteLine("\n=== Get Monitoring List ===");
            Console.Write("Enter Warehouse Name (leave empty for all): ");
            string warehouseName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(warehouseName))
            {
               warehouseName = null;
            }

            DateTime? expiredDate = null;
            Console.Write("Enter Expired Date (YYYY-MM-DD) (leave empty for all): ");
            string expiredDateString = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(expiredDateString))
            {
               expiredDate = DateTime.Now;
            }
            else
            {
               expiredDate = DateTime.Parse(expiredDateString);
            }

            var monitoringList = monitoringService.GetMonitoringList(warehouseName, expiredDate ?? DateTime.Now);

            if (monitoringList != null && monitoringList.Any())
            {
               Console.WriteLine("\nMonitoring List:");
               foreach (var item in monitoringList)
               {
                  Console.WriteLine($"{item.NamaBarang} - {item.TanggalKadaluarsa:yyyy-MM-dd}");
               }
            }
            else
            {
               Console.WriteLine("No items found matching the criteria.");
            }
         }
      }
   }
