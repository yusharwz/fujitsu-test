/* Buat 2 Tabel (Tabel Gudang & Tabel Barang) */
CREATE TABLE Gudang (
    KodeGudang SERIAL PRIMARY KEY,
    NamaGudang VARCHAR(100) NOT NULL
);

CREATE TABLE Barang (
    KodeBarang SERIAL PRIMARY KEY,
    NamaBarang VARCHAR(100) NOT NULL,
    HargaBarang NUMERIC(10, 2) NOT NULL,
    JumlahBarang INT NOT NULL,
    TanggalKadaluarsa DATE NOT NULL,
    KodeGudang INT,
    FOREIGN KEY (KodeGudang) REFERENCES Gudang(KodeGudang)
);

/* Function untuk Dynamic Query dan Paging */
CREATE OR REPLACE FUNCTION GetItemPaging(
    p_nomor_halaman INT,
    p_ukuran_halaman INT
)
RETURNS TABLE (
    KodeGudang INT,
    NamaGudang VARCHAR,
    KodeBarang INT,
    NamaBarang VARCHAR,
    HargaBarang NUMERIC,
    JumlahBarang INT,
    TanggalKadaluarsa DATE
) AS $$
BEGIN
    RETURN QUERY
    SELECT g.KodeGudang, g.NamaGudang, b.KodeBarang, b.NamaBarang, b.HargaBarang, b.JumlahBarang, b.TanggalKadaluarsa
    FROM Gudang g
    JOIN Barang b ON g.KodeGudang = b.KodeGudang
    ORDER BY b.KodeBarang
    OFFSET (p_nomor_halaman - 1) * p_ukuran_halaman
    FETCH NEXT p_ukuran_halaman ROWS ONLY;
END;
$$ LANGUAGE plpgsql;

/* Trigger untuk Pengecekan Barang Kadaluarsa */
CREATE OR REPLACE FUNCTION trg_periksa_kadaluarsa()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.TanggalKadaluarsa <= CURRENT_DATE THEN
        RAISE NOTICE 'Barang yang kedaluwarsa telah ditambahkan!';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER cek_kadaluarsa_trigger
AFTER INSERT ON Barang
FOR EACH ROW
EXECUTE FUNCTION trg_periksa_kadaluarsa();


/* Data Dummy */
INSERT INTO Gudang (NamaGudang) VALUES
('Gudang A'),
('Gudang B'),
('Gudang C'),
('Gudang D'),
('Gudang E'),
('Gudang F'),
('Gudang G'),
('Gudang H'),
('Gudang I'),
('Gudang J');

INSERT INTO Barang (NamaBarang, HargaBarang, JumlahBarang, TanggalKadaluarsa, KodeGudang) 
VALUES 
    ('Pena', 1.50, 100, '2024-12-31', 1), 
    ('Buku', 5.75, 50, '2025-06-30', 2), 
    ('Pensil', 0.75, 200, '2024-08-15', 3), 
    ('Penghapus', 0.50, 150, '2024-09-30', 1), 
    ('Bolpoin', 2.00, 80, '2025-01-31', 4), 
    ('Kertas', 4.25, 1000, '2024-12-01', 2), 
    ('Cutter', 3.50, 30, '2024-07-31', 5), 
    ('Stapler', 8.50, 25, '2024-11-15', 3), 
    ('Gunting', 2.75, 60, '2024-10-10', 2), 
    ('Lem', 1.25, 120, '2024-09-01', 1);
