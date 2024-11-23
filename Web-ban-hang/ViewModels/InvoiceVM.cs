namespace Web_ban_hang.ViewModels
{
	public class InvoiceVM
	{
		public int MaCT {  get; set; }
		public int MaHD { get; set; }	
		public DateTime NgayDat { get; set; }
		public DateTime NgayCan { get; set; }
		public DateTime NgayGiao { get; set; }
		public string MaKH { get; set; }
		public string HoTen { get; set; }
		public string DiaChi { get; set; }
		public string CachThanhToan { get; set; }
		public string GhiChu { get; set; }
		public int TrangThai { get; set; }
		public double TongTien { get; set; }
        public string TenSP { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
    }

    public class InvoiceDetailVM
	{
		public string TenSP { get; set; }	
        public int MaCt { get; set; }
        public int MaHd { get; set; }
        public int MaHh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double GiamGia { get; set; }
    }
}
