using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string TenHH { get; set; }
        [Required]
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }   
        public string Hinh { get; set; }
        [Required]
        public double DonGia { get; set;}
        public string ChiTiet {  get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; } 
        public int MaLoai { get; set; }
        public bool TrangThai { get; set; }
		public DateTime NgaySx { get; set; }
		public string MaNcc { get; set; } = null!;
		public double GiamGia { get; set; }
		public int SoLanXem { get; set; }

	}
}
