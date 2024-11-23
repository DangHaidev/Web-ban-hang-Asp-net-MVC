using System;
using System.Collections.Generic;

namespace Web_ban_hang.Models.Entities
{
    public partial class NhaCungCap
    {
        public NhaCungCap()
        {
            HangHoas = new HashSet<HangHoa>();
        }

        public string MaNcc { get; set; } = null!;
        public string TenCongTy { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string? NguoiLienLac { get; set; }
        public string Email { get; set; } = null!;
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? MoTa { get; set; }

        public virtual ICollection<HangHoa> HangHoas { get; set; }
    }
}
