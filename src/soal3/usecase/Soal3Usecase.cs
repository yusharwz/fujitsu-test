using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagement
{
   public class Usecase
   {
      public static void GetWarehouse(WarehouseRepo warehouseService)
      {
         Console.Write("Masukkan Kode Gudang (biarkan kosong untuk menampilkan semua): ");
         string input = Console.ReadLine();

         if (string.IsNullOrWhiteSpace(input))
         {
            var warehouses = warehouseService.GetAllWarehouses();
            foreach (var warehouse in warehouses)
            {
               Console.WriteLine($"Kode Gudang: {warehouse.KodeGudang}, Nama Gudang: {warehouse.NamaGudang}");
            }
         }
         else if (int.TryParse(input, out int kodeGudang))
         {
            var warehouse = warehouseService.GetWarehouse(kodeGudang);
            if (warehouse != null)
            {
               Console.WriteLine($"Kode Gudang: {warehouse.KodeGudang}, Nama Gudang: {warehouse.NamaGudang}");
            }
            else
            {
               Console.WriteLine("Gudang tidak ditemukan.");
            }
         }
         else
         {
            Console.WriteLine("Kode Gudang tidak valid.");
         }
      }

      public static void AddWarehouse(WarehouseRepo warehouseService)
      {
         Console.WriteLine("\n=== Tambah Gudang ===");

         string namaGudang;
         do
         {
            Console.Write("Masukkan Nama Gudang: ");
            namaGudang = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(namaGudang))
            {
               Console.WriteLine("Nama Gudang tidak boleh kosong. Silakan masukkan nama yang valid.");
            }
         } while (string.IsNullOrWhiteSpace(namaGudang));

         var gudangBaru = new Gudang { NamaGudang = namaGudang };
         warehouseService.AddWarehouse(gudangBaru);

         var gudang = warehouseService.GetWarehouse(gudangBaru.KodeGudang);
         if (gudang != null)
         {
            Console.WriteLine($"Gudang ditambahkan: Kode Gudang: {gudang.KodeGudang}, Nama Gudang: {gudang.NamaGudang}");
         }
         else
         {
            Console.WriteLine("Gudang tidak ditemukan atau tidak dapat diambil.");
         }
      }

      public static void UpdateWarehouse(WarehouseRepo warehouseService)
      {
         Console.WriteLine("\n=== Update Gudang ===");
         Console.Write("Masukkan Kode Gudang yang akan diupdate: ");
         int kodeGudang;
         while (!int.TryParse(Console.ReadLine(), out kodeGudang))
         {
            Console.WriteLine("Kode gudang tidak valid. Silakan masukkan angka yang valid.");
            Console.Write("Masukkan Kode Gudang yang akan diupdate: ");
         }

         var gudang = warehouseService.GetWarehouse(kodeGudang);
         if (gudang == null)
         {
            Console.WriteLine($"Gudang dengan kode {kodeGudang} tidak ditemukan.");
            return;
         }

         string namaGudangBaru;
         do
         {
            Console.Write("Masukkan Nama Gudang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
            namaGudangBaru = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(namaGudangBaru))
            {
               namaGudangBaru = gudang.NamaGudang;
            }
         } while (string.IsNullOrWhiteSpace(namaGudangBaru));

         gudang.NamaGudang = namaGudangBaru;
         warehouseService.UpdateWarehouse(gudang);

         Console.WriteLine("Gudang berhasil diupdate.");
      }

      public static void RemoveWarehouse(WarehouseRepo warehouseService)
      {
         Console.WriteLine("\n=== Hapus Gudang ===");
         int kodeGudang;

         do
         {
            Console.Write("Masukkan Kode Gudang yang akan dihapus: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out kodeGudang))
            {
               Console.WriteLine("Kode gudang tidak valid. Silakan masukkan angka yang valid.");
            }
            else
            {
               var gudang = warehouseService.GetWarehouse(kodeGudang);
               if (gudang == null)
               {
                  Console.WriteLine($"Gudang dengan kode {kodeGudang} tidak ditemukan.");
                  return;
               }
            }
         } while (kodeGudang <= 0);

         warehouseService.RemoveWarehouse(kodeGudang);
         Console.WriteLine($"Gudang berhasil dihapus.");
      }

      public static void AddItem(ItemRepo itemService, WarehouseRepo warehouseService)
      {
         Console.WriteLine("\n=== Tambah Barang ===");

         string namaBarang;
         decimal hargaBarang;
         int jumlahBarang;
         DateTime tanggalKadaluarsa;
         int kodeGudang;

         do
         {
            Console.Write("Masukkan Nama Barang: ");
            namaBarang = Console.ReadLine();
         } while (string.IsNullOrWhiteSpace(namaBarang));

         do
         {
            Console.Write("Masukkan Harga Barang: ");
            string inputHarga = Console.ReadLine();
            if (!decimal.TryParse(inputHarga, out hargaBarang))
            {
               Console.WriteLine("Harga barang tidak valid. Silakan masukkan angka desimal yang valid.");
            }
         } while (hargaBarang <= 0);

         do
         {
            Console.Write("Masukkan Jumlah Barang: ");
            string inputJumlah = Console.ReadLine();
            if (!int.TryParse(inputJumlah, out jumlahBarang))
            {
               Console.WriteLine("Jumlah barang tidak valid. Silakan masukkan angka integer yang valid.");
            }
         } while (jumlahBarang <= 0);

         do
         {
            Console.Write("Masukkan Tanggal Kadaluarsa (YYYY-MM-DD): ");
            string inputTanggalKadaluarsa = Console.ReadLine();
            if (!DateTime.TryParse(inputTanggalKadaluarsa, out tanggalKadaluarsa))
            {
               Console.WriteLine("Format tanggal tidak valid. Silakan masukkan tanggal dalam format YYYY-MM-DD yang benar.");
            }
         } while (tanggalKadaluarsa == DateTime.MinValue);

         bool isValidGudang = false;
         do
         {
            Console.Write("Masukkan Kode Gudang: ");
            string inputKodeGudang = Console.ReadLine();
            if (!int.TryParse(inputKodeGudang, out kodeGudang))
            {
               Console.WriteLine("Kode gudang tidak valid. Silakan masukkan angka integer yang valid.");
               continue;
            }

            var gudang = warehouseService.GetWarehouse(kodeGudang);
            if (gudang == null)
            {
               Console.WriteLine($"Gudang dengan kode {kodeGudang} tidak ada.");
            }
            else
            {
               isValidGudang = true;
            }
         } while (!isValidGudang);

         var barangBaru = new Barang
         {
            NamaBarang = namaBarang,
            HargaBarang = hargaBarang,
            JumlahBarang = jumlahBarang,
            TanggalKadaluarsa = tanggalKadaluarsa,
            KodeGudang = kodeGudang
         };

         itemService.AddItem(barangBaru);
         Console.WriteLine($"Barang berhasil ditambahkan.");
      }

      public static void GetItem(ItemRepo itemService)
      {
         Console.Write("Masukkan Kode Barang (biarkan kosong untuk menampilkan semua): ");
         string input = Console.ReadLine();

         if (string.IsNullOrWhiteSpace(input))
         {
            var items = itemService.GetAllItems();
            var groupedItems = new Dictionary<int, List<Barang>>();

            foreach (var item in items)
            {
               if (!groupedItems.ContainsKey(item.KodeGudang))
               {
                  groupedItems[item.KodeGudang] = new List<Barang>();
               }
               groupedItems[item.KodeGudang].Add(item);
            }

            foreach (var group in groupedItems)
            {
               var namaGudang = group.Value[0].NamaGudang;
               Console.WriteLine("\n---------------------");
               Console.WriteLine($"Nama Gudang: {namaGudang}");
               Console.WriteLine("Daftar Barang:");
               foreach (var item in group.Value)
               {
                  Console.WriteLine($"- {item.NamaBarang} ({item.JumlahBarang} pcs)");
               }
               Console.WriteLine("---------------------");
            }
         }
         else if (int.TryParse(input, out int kodeBarang))
         {
            var item = itemService.GetItem(kodeBarang);
            if (item != null)
            {
               Console.WriteLine($"Kode Barang: {item.KodeBarang}");
               Console.WriteLine($"Nama Barang: {item.NamaBarang}");
               Console.WriteLine($"Harga Barang: {item.HargaBarang}");
               Console.WriteLine($"Jumlah Barang: {item.JumlahBarang}");
               Console.WriteLine($"Tanggal Kadaluarsa: {item.TanggalKadaluarsa.ToString("yyyy-MM-dd")}");
               Console.WriteLine($"Nama Gudang: {item.NamaGudang}");
            }
            else
            {
               Console.WriteLine("Barang tidak ditemukan.");
            }
         }
         else
         {
            Console.WriteLine("Kode Barang tidak valid.");
         }
      }

      public static void UpdateItem(ItemRepo itemService, WarehouseRepo warehouseService)
      {
         Console.WriteLine("\n=== Update Barang ===");
         Console.Write("Masukkan Kode Barang yang akan diupdate: ");
         int kodeBarang;
         while (!int.TryParse(Console.ReadLine(), out kodeBarang))
         {
            Console.WriteLine("Input tidak valid. Silakan masukkan Kode Barang yang valid.");
            Console.Write("Masukkan Kode Barang yang akan diupdate: ");
         }

         var item = itemService.GetItem(kodeBarang);
         if (item == null)
         {
            Console.WriteLine($"Barang dengan kode {kodeBarang} tidak ditemukan.");
            return;
         }

         Console.Write("Masukkan Nama Barang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
         string namaBarangBaru = Console.ReadLine();

         decimal hargaBarangBaru = 0;
         Console.Write("Masukkan Harga Barang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
         string inputHarga = Console.ReadLine();
         if (!string.IsNullOrWhiteSpace(inputHarga))
         {
            while (!decimal.TryParse(inputHarga, out hargaBarangBaru) || hargaBarangBaru <= 0)
            {
               Console.WriteLine("Input tidak valid. Harga Barang harus berupa angka positif.");
               Console.Write("Masukkan Harga Barang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
               inputHarga = Console.ReadLine();
            }
         }
         else
         {
            hargaBarangBaru = item.HargaBarang;
         }

         int jumlahBarangBaru = 0;
         Console.Write("Masukkan Jumlah Barang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
         string inputJumlah = Console.ReadLine();
         if (!string.IsNullOrWhiteSpace(inputJumlah))
         {
            while (!int.TryParse(inputJumlah, out jumlahBarangBaru) || jumlahBarangBaru <= 0)
            {
               Console.WriteLine("Input tidak valid. Jumlah Barang harus berupa angka positif.");
               Console.Write("Masukkan Jumlah Barang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
               inputJumlah = Console.ReadLine();
            }
         }
         else
         {
            jumlahBarangBaru = item.JumlahBarang;
         }

         DateTime? tanggalKadaluarsaBaru = null;
         Console.Write("Masukkan Tanggal Kadaluarsa baru (YYYY-MM-DD) (biarkan kosong untuk mempertahankan nilai saat ini): ");
         string inputTanggalKadaluarsa = Console.ReadLine();
         if (!string.IsNullOrWhiteSpace(inputTanggalKadaluarsa))
         {
            while (!DateTime.TryParse(inputTanggalKadaluarsa, out DateTime tempDate))
            {
               Console.WriteLine("Input tidak valid. Silakan masukkan tanggal dalam format YYYY-MM-DD yang benar.");
               Console.Write("Masukkan Tanggal Kadaluarsa baru (YYYY-MM-DD) (biarkan kosong untuk mempertahankan nilai saat ini): ");
               inputTanggalKadaluarsa = Console.ReadLine();
            }
            tanggalKadaluarsaBaru = DateTime.Parse(inputTanggalKadaluarsa);
         }
         else
         {
            tanggalKadaluarsaBaru = item.TanggalKadaluarsa;
         }

         int kodeGudangBaru = 0;
         Console.Write("Masukkan Kode Gudang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
         string inputKodeGudang = Console.ReadLine();
         if (!string.IsNullOrWhiteSpace(inputKodeGudang))
         {
            while (!int.TryParse(inputKodeGudang, out kodeGudangBaru) || kodeGudangBaru <= 0)
            {
               Console.WriteLine("Input tidak valid. Kode Gudang harus berupa angka positif.");
               Console.Write("Masukkan Kode Gudang baru (biarkan kosong untuk mempertahankan nilai saat ini): ");
               inputKodeGudang = Console.ReadLine();
            }
         }
         else
         {
            kodeGudangBaru = item.KodeGudang;
         }

         if (!string.IsNullOrWhiteSpace(namaBarangBaru))
         {
            item.NamaBarang = namaBarangBaru;
         }

         if (hargaBarangBaru > 0)
         {
            item.HargaBarang = hargaBarangBaru;
         }

         if (jumlahBarangBaru > 0)
         {
            item.JumlahBarang = jumlahBarangBaru;
         }

         if (tanggalKadaluarsaBaru != null)
         {
            item.TanggalKadaluarsa = tanggalKadaluarsaBaru.Value;
         }

         if (kodeGudangBaru > 0)
         {
            var gudang = warehouseService.GetWarehouse(kodeGudangBaru);
            if (gudang == null)
            {
               Console.WriteLine($"Gudang dengan kode {kodeGudangBaru} tidak ditemukan. Kode Gudang Barang tetap tidak berubah.");
            }
            else
            {
               item.KodeGudang = kodeGudangBaru;
            }
         }

         itemService.UpdateItem(item);
         Console.WriteLine($"Barang berhasil diupdate.");
      }

      public static void RemoveItem(ItemRepo itemService)
      {
         Console.WriteLine("\n=== Hapus Barang ===");
         Console.Write("Masukkan Kode Barang yang akan dihapus: ");
         int kodeBarang = Convert.ToInt32(Console.ReadLine());

         var item = itemService.GetItem(kodeBarang);
         if (item == null)
         {
            Console.WriteLine($"Barang dengan kode {kodeBarang} tidak ditemukan.");
            return;
         }

         itemService.RemoveItem(kodeBarang);
         Console.WriteLine($"Barang berhasil dihapus.");
      }

      public static void GetMonitoringList(MonitoringRepo monitoringService)
      {
         Console.WriteLine("\nCATATAN: Daftar barang yang ditampilkan adalah barang-barang yang tanggal kadaluarsanya sudah melewati tanggal yang dimasukkan atau tinggal 14 hari lagi sebelum kadaluarsa.");
         Console.Write("\nMasukkan Nama Gudang (biarkan kosong untuk menampilkan semua): ");
         string namaGudang = Console.ReadLine();

         Console.Write("Masukkan Tanggal Referensi (yyyy-MM-dd) (biarkan kosong untuk tanggal hari ini): ");
         string dateInput = Console.ReadLine();
         DateTime tanggalReferensi;

         if (string.IsNullOrEmpty(dateInput))
         {
            tanggalReferensi = DateTime.Today;
         }
         else if (!DateTime.TryParse(dateInput, out tanggalReferensi))
         {
            Console.WriteLine("Format tanggal tidak valid.");
            return;
         }

         var items = monitoringService.GetMonitoringList(string.IsNullOrEmpty(namaGudang) ? null : namaGudang, tanggalReferensi);

         if (items == null || !items.Any())
         {
            Console.WriteLine("Data tidak ditemukan.");
            return;
         }

         foreach (var warehouseGroup in items.GroupBy(i => new { i.KodeGudang, i.NamaGudang }))
         {
            Console.WriteLine($"\nKode Gudang: {warehouseGroup.Key.KodeGudang ?? "N/A"}");
            Console.WriteLine($"Nama Gudang: {warehouseGroup.Key.NamaGudang ?? "N/A"}");
            Console.WriteLine("Daftar Barang Kadaluarsa:");
            Console.WriteLine("  --------------------------------------------------------");

            foreach (var item in warehouseGroup)
            {
               var daysToExpiry = (item.TanggalKadaluarsa - tanggalReferensi).Days;
               string expiryStatus = daysToExpiry < 0 ? "(sudah kadaluarsa)" : $"({daysToExpiry} hari lagi kadaluarsa)";

               Console.WriteLine($"  Kode Barang: {item.KodeBarang ?? "N/A"}");
               Console.WriteLine($"  Nama Barang: {item.NamaBarang ?? "N/A"}");
               Console.WriteLine($"  Jumlah: {item.JumlahBarang}");
               Console.WriteLine($"  Tanggal Kadaluarsa: {item.TanggalKadaluarsa:yyyy-MM-dd} {expiryStatus}");
               Console.WriteLine("  --------------------------------------------------------");
            }
            Console.WriteLine();
         }
      }
   }
}
