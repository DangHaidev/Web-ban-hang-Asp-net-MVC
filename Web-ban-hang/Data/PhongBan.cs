using System;
using System.Collections.Generic;

namespace Web_ban_hang.Data
{
    public partial class PhongBan
    {
        public PhongBan()
        {
            PhanCongs = new HashSet<PhanCong>();
            PhanQuyens = new HashSet<PhanQuyen>();
        }

        public string MaPb { get; set; } = null!;
        public string TenPb { get; set; } = null!;
        public string? ThongTin { get; set; }

        public virtual ICollection<PhanCong> PhanCongs { get; set; }
        public virtual ICollection<PhanQuyen> PhanQuyens { get; set; }
    }
}
