using System;
using System.Collections.Generic;

namespace Web_ban_hang.Models.Entities
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            ChuDes = new HashSet<ChuDe>();
            HoaDons = new HashSet<HoaDon>();
            HoiDaps = new HashSet<HoiDap>();
            PhanCongs = new HashSet<PhanCong>();
        }

        public string MaNv { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? MatKhau { get; set; }

        public virtual ICollection<ChuDe> ChuDes { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<HoiDap> HoiDaps { get; set; }
        public virtual ICollection<PhanCong> PhanCongs { get; set; }
    }
}
