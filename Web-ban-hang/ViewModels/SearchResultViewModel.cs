namespace Web_ban_hang.ViewModels
{
    public class SearchResultViewModel
    {
        public string Type { get; set; } // Loại dữ liệu: Product, Category, ...
        public string Title { get; set; } // Tên hoặc tiêu đề
        public string Description { get; set; } // Mô tả ngắn gọn
        public string Link { get; set; } // Đường dẫn chi tiết
        public string srcImage { get; set; }
    }
}
