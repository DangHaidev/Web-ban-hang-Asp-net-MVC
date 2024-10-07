using System;
using System.Collections.Generic;

namespace Web_ban_hang.Data
{
    public partial class GopY
    {
        public string MaGy { get; set; } = null!;
        public int MaCd { get; set; }
        public string NoiDung { get; set; } = null!;
        public DateTime NgayGy { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? DienThoai { get; set; }
        public bool CanTraLoi { get; set; }
        public string? TraLoi { get; set; }
        public DateTime? NgayTl { get; set; }

        public virtual ChuDe MaCdNavigation { get; set; } = null!;
    }
}
