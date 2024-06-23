namespace WarehouseManagement
{
   public class Barang
   {
      public int KodeBarang { get; set; }
      public string NamaBarang { get; set; }
      public decimal HargaBarang { get; set; }
      public int JumlahBarang { get; set; }
      public DateTime TanggalKadaluarsa { get; set; }
      public int KodeGudang { get; set; }
      public string NamaGudang { get; set; }
   }

   public class Gudang
   {
      public int KodeGudang { get; set; }
      public string NamaGudang { get; set; }
   }

}