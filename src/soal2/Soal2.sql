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
INSERT INTO Gudang (NamaGudang)
VALUES
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
('Pena', 1.50, 100, '2020-12-31', 1), 
('Buku', 5.75, 50, '2025-06-30', 2), 
('Pensil', 0.75, 200, '2024-08-15', 3), 
('Penghapus', 0.50, 150, '2024-09-30', 1), 
('Bolpoin', 2.00, 80, '2025-01-31', 4), 
('Kertas', 4.25, 1000, '2024-12-01', 2), 
('Cutter', 3.50, 30, '2024-07-31', 5), 
('Stapler', 8.50, 25, '2024-11-15', 3), 
('Gunting', 2.75, 60, '2024-10-10', 2), 
('Lem', 1.25, 120, '2024-09-01', 1),
('Pencil Case', 3.00, 40, '2024-11-30', 6),
('Ruler', 1.75, 90, '2025-02-28', 7),
('Highlighter', 1.50, 70, '2024-10-15', 8),
('Eraser', 0.50, 200, '2024-08-31', 9),
('Notebook', 6.00, 60, '2024-12-31', 1),
('Folder', 2.50, 100, '2024-09-15', 2),
('Markers', 4.00, 50, '2024-11-30', 3),
('Glue Stick', 1.75, 80, '2025-03-31', 4),
('Scissors', 3.25, 30, '2020-07-15', 5),
('Paper Clips', 0.75, 300, '2024-09-30', 6),
('Staples', 2.00, 150, '2025-01-31', 7),
('Tape', 1.25, 120, '2024-12-15', 8),
('Sharpener', 0.75, 200, '2024-08-31', 9),
('Desk Organizer', 5.50, 35, '2024-07-07', 1),
('Whiteboard', 12.00, 20, '2025-04-30', 2),
('Calculator', 9.00, 15, '2024-10-31', 3),
('Bookends', 3.75, 40, '2024-12-15', 4),
('Hole Punch', 4.50, 25, '2024-09-30', 5),
('Rubber Bands', 0.50, 500, '2024-07-01', 6),
('Index Cards', 1.00, 250, '2020-12-31', 7),
('Binder Clips', 1.50, 200, '2025-02-28', 8),
('Correction Fluid', 2.25, 70, '2024-11-15', 9),
('Desk Lamp', 15.00, 10, '2025-05-31', 1),
('File Folders', 3.50, 60, '2020-10-31', 2),
('USB Drive', 8.00, 30, '2024-07-05', 3),
('Writing Pad', 2.25, 100, '2024-09-30', 4),
('Post-it Notes', 1.00, 150, '2025-02-28', 5),
('Binder', 4.50, 45, '2024-11-15', 6),
('Push Pins', 0.75, 200, '2024-09-30', 7),
('Book Stand', 6.75, 20, '2024-07-03', 8),
('Desk Calendar', 3.25, 80, '2024-12-15', 9),
('Desk Chair', 50.00, 5, '2020-06-30', 1),
('Magnifying Glass', 7.50, 15, '2024-10-31', 2),
('Bulletin Board', 10.00, 10, '2025-01-15', 3),
('Drawer Organizer', 4.75, 30, '2024-09-30', 4),
('Laptop Stand', 12.50, 20, '2025-02-28', 5),
('Wall Clock', 9.25, 25, '2024-11-15', 6),
('Coat Rack', 15.75, 8, '2024-10-31', 7),
('Filing Cabinet', 80.00, 2, '2025-03-31', 8),
('Standing Desk', 200.00, 1, '2024-12-15', 9),
('Desk Mat', 5.25, 40, '2020-02-28', 1),
('Office Plants', 3.00, 50, '2024-11-15', 2),
('Desk Fan', 20.00, 6, '2024-10-31', 3),
('Wall Organizer', 6.25, 30, '2025-01-15', 4),
('Bookcase', 70.00, 3, '2024-09-30', 5),
('Desk Clock', 4.50, 25, '2025-02-28', 6),
('Memo Board', 2.75, 70, '2024-12-15', 7),
('Waste Basket', 3.25, 40, '2024-10-31', 8),
('Desk Heater', 25.00, 5, '2025-03-31', 9),
('Wall Shelf', 15.50, 12, '2024-11-15', 1),
('Paper Shredder', 30.00, 3, '2025-01-15', 2),
('Desk Organizer Set', 8.75, 20, '2024-09-30', 3),
('Desk Drawer Organizer', 6.00, 25, '2025-02-28', 4),
('Label Maker', 9.50, 15, '2024-12-15', 5),
('Desk Accessories Set', 12.75, 10, '2024-10-31', 6),
('Desk Name Plate', 2.00, 50, '2025-03-31', 7),
('Wall Calendar', 7.00, 15, '2024-11-15', 8),
('Office Chair Mat', 18.50, 8, '2025-01-15', 9),
('Mail Organizer', 4.25, 35, '2024-09-30', 1),
('Business Card Holder', 1.50, 60, '2025-02-28', 2),
('Laptop Bag', 15.00, 10, '2024-12-15', 3),
('Desktop Organizer', 12.25, 12, '2024-10-31', 4),
('Bulletin Board Set', 8.50, 18, '2025-03-31', 5),
('Wall Mount File Organizer', 10.75, 5, '2024-11-15', 6),
('Desk Hutch', 25.00, 4, '2025-01-15', 7),
('Message Board', 3.75, 30, '2024-09-30', 8),
('Desk Caddy', 5.00, 25, '2025-02-28', 9),
('Wall Grid Panel', 20.00, 3, '2024-12-15', 1),
('Monitor Stand', 18.25, 6, '2024-10-31', 2),
('Floor Mat', 30.00, 2, '2025-03-31', 3),
('Desk Lamp Set', 22.50, 5, '2024-11-15', 4),
('Desk Pedestal', 50.00, 1, '2025-01-15', 5);