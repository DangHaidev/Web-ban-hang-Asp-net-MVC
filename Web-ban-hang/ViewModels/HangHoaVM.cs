namespace Web_ban_hang.ViewModels
{
    public class HangHoaVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set;}
    }
    public class ChiTietHangHoaVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set;}
        public string ChiTiet {  get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; } 
        public int MaLoai { get; set; }
    }
}
