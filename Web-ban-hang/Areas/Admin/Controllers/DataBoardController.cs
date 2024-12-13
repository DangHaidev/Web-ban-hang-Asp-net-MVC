using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Web_ban_hang.Helpers;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.Services;
using Web_ban_hang.ViewModels;
using X.PagedList.Extensions;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DataBoardController : Controller
    {
        private readonly HDangShopContext db;
        private readonly IMapper _mapper;


        public DataBoardController(HDangShopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }


        public IActionResult Index()
        {
            var data = db.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHdNavigation.MaKhNavigation)
               .OrderByDescending(o => o.MaHdNavigation.NgayDat)
               .ToList()
               .Where(p => p.MaHdNavigation.MaTrangThai == 1);

            ViewBag.Todaysale = db.ChiTietHds.Include(p => p.MaHdNavigation).Where(p => p.MaHdNavigation.NgayDat.Date == DateTime.Now.Date).Sum(p => p.SoLuong * p.DonGia);
            ViewBag.Totalsale = db.ChiTietHds.Include(p => p.MaHdNavigation).Sum(p => (p.SoLuong * p.DonGia) * (1 - p.GiamGia));
            ViewBag.SoHH = db.ChiTietHds.Include(p => p.MaHdNavigation).Where(p => p.MaHdNavigation.NgayDat.Date == DateTime.Now.Date).Sum(p => p.SoLuong);
            ViewBag.TotalOrder = db.HoaDons.Where(p => p.NgayDat.Date == DateTime.Now.Date).Count();


            var result = data.Select(item => new InvoiceVM
            {
                MaHD = item.MaHd,
                HoTen = item.MaHdNavigation.MaKhNavigation.HoTen,
                TongTien = (item.DonGia * item.SoLuong) * (1 - item.GiamGia),
                NgayDat = item.MaHdNavigation.NgayDat,
                TrangThai = item.MaHdNavigation.MaTrangThai,
            }).ToList().DistinctBy(p => p.MaHD).Take(5);





			var revenueData = db.ChiTietHds
	.Where(ct => ct.MaHdNavigation != null) // Đảm bảo có hóa đơn
	.GroupBy(ct => ct.MaHdNavigation.NgayDat.Date)
	.Select(g => new
	{
		Ngay = g.Key,
		DoanhThu = g.Sum(ct => ct.SoLuong * ct.DonGia) // Tính tổng doanh thu từ chi tiết hóa đơn
	}).Take(7)
	.OrderByDescending(r => r.Ngay)
	.ToList();

			// Chuyển dữ liệu sang JSON để sử dụng trong JavaScript
			var labels = revenueData.Select(r => r.Ngay.ToString("yyyy-MM-dd")).ToList();
            var dataChart = revenueData.Select(r => r.DoanhThu).ToList();

            // Gửi dữ liệu về View
            ViewBag.Labels = labels;
            ViewBag.Data = dataChart;



			var MonthrevenueData = db.ChiTietHds
	.Join(db.HoaDons, ct => ct.MaHd, hd => hd.MaHd, (ct, hd) => new { ct, hd })  // Thực hiện Join với HoaDon
	.Where(x => x.hd.NgayDat != null) // Đảm bảo có ngày đặt hàng
	.GroupBy(x => new { Year = x.hd.NgayDat.Year, Month = x.hd.NgayDat.Month }) // Nhóm theo năm và tháng
	.Select(g => new
	{
		Ngay = $"{g.Key.Month}/{g.Key.Year}", // Tạo chuỗi "MM/yyyy"
		DoanhThu = g.Sum(x => x.ct.SoLuong * x.ct.DonGia) // Tính doanh thu
	})
	/*.OrderByDescending(r => r.Ngay)*/ // Sắp xếp theo "MM/yyyy" giảm dần
	.Take(12) // Lấy 12 kết quả gần nhất
	.ToList();

			// Chuyển dữ liệu sang JSON để sử dụng trong JavaScript
			var labelsChart2 = MonthrevenueData.Select(r => r.Ngay).ToList();
			var dataChart2 = MonthrevenueData.Select(r => r.DoanhThu).ToList();

			// Gửi dữ liệu về View
			ViewBag.LabelsC2 = labelsChart2;
			ViewBag.DataC2 = dataChart2;




			return View(result);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không có admin này");
                        TempData["Error"] = "Tài khoản khong hợp lệ. ";

                }
                else
                {
                    if (!khachHang.HieuLuc || khachHang.VaiTro != 3)
                    {
                        ModelState.AddModelError("loi", "Tài khoản khong hợp lệ. ");
                        TempData["Error"] = "Tài khoản khong hợp lệ. ";
                    }
                    else
                    {
                        if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey) )
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                        }
                        else
                        {
                            return Redirect("Index");
                        }
                    }
                }
            }

            return View();
        }

        public IActionResult InvoiceDetail(int? page)
        {
            var data = db.HoaDons.Include(p => p.MaKhNavigation)
                .OrderByDescending(o => o.NgayDat)
                .ToList();

            var result = data.Select(item => new InvoiceVM
            {
                MaHD = item.MaHd,
                HoTen = item.MaKhNavigation.HoTen,
                NgayCan = item.NgayCan ?? item.NgayDat,
                NgayDat = item.NgayDat,
                TrangThai = item.MaTrangThai,
            });

            // Thực hiện phân trang
            int pageSize = 20; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1
            var pagedResult = result.ToPagedList(pageNumber, pageSize);

            ViewBag.page = page;

            return View(pagedResult);
        }

        public IActionResult ViewOrder(int id)
        {
            var cthd = db.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).Where(p => p.MaHd == id).ToList();

            ViewBag.MaHD = id;
            //ViewBag.TenKH = db.ChiTietHds.Where(p => p.MaHd == id).FirstOrDefault().MaHdNavigation.HoTen;

            var chiTiet = db.HoaDons
    .Where(p => p.MaHd == id)
    .Include(p => p.MaKhNavigation)
    .FirstOrDefault();

            ViewBag.HoaDons = chiTiet;

            if (chiTiet != null && chiTiet.MaKhNavigation != null)
            {
                ViewBag.TenKH = chiTiet.MaKhNavigation.HoTen;
            }
            else
            {
                ViewBag.TenKH = "NULL";
            }


            var result = cthd.Select(data => new InvoiceVM
            {
                MaCT = data.MaCt,
                MaHD = data.MaHd,
                NgayDat = data.MaHdNavigation.NgayDat,
                MaKH = data.MaHdNavigation.MaKh,
                HoTen = data.MaHdNavigation.HoTen,
                DiaChi = data.MaHdNavigation.DiaChi,
                CachThanhToan = data.MaHdNavigation.CachThanhToan,
                GhiChu = data.MaHdNavigation.GhiChu,
                TrangThai = data.MaHdNavigation.MaTrangThai,
                TenSP = data.MaHhNavigation.TenHh,
                SoLuong = data.SoLuong,
                DonGia = data.DonGia,
                GiamGia = data.GiamGia,
            }).ToList();



            return View(result);
        }

        [HttpPost]
        public IActionResult ViewOrder(InvoiceVM model)
        {
            var data = db.ChiTietHds.Include(p => p.MaHdNavigation).SingleOrDefault(p => p.MaCt == model.MaCT);

            //data.MaHdNavigation.HoTen = model.HoTen;
            data.SoLuong = model.SoLuong;
            data.DonGia = model.DonGia;
            data.MaHdNavigation.MaTrangThai = model.TrangThai;
            data.MaHdNavigation.GhiChu = model.GhiChu;

            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Product()
        {
            return RedirectToAction("Index");
        }
        public IActionResult Customer(int? page)
        {

            var customers = db.KhachHangs.ToList();

            var customerViewModels = _mapper.Map<List<Customer>>(customers);


            // Thực hiện phân trang
            int pageSize = 10; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1
            var pagedResult = customerViewModels.ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }



        [HttpGet]
        public async Task<IActionResult> SearchAdmin(string? keyword)
        {

            if (string.IsNullOrEmpty(keyword))
                return View(new List<SearchResultViewModel>());

            // Tìm kiếm trong bảng Products
            var productResults = await db.HangHoas
                .Where(p => p.TenHh.Contains(keyword) || p.MoTa.Contains(keyword))
                .Select(p => new SearchResultViewModel
                {
                    Type = "Hàng hóa",
                    Title = p.TenHh,
                    Description = p.MoTa,
                    Link = $"/Admin/Product/Detail/{p.MaHh}",
                    srcImage = $"/Hinh/HangHoa/{p.Hinh}",
                }).ToListAsync();

            // Tìm kiếm trong bảng Categories
            var cusResult = await db.KhachHangs
                .Where(c => c.MaKh.Contains(keyword) || c.HoTen.Contains(keyword))
                .Select(c => new SearchResultViewModel
                {
                    Type = "Khách hàng",
                    Title = c.MaKh,
                    Description = c.HoTen,
                    Link = $"/Admin/Customer/Edit/{c.MaKh}",
                    srcImage = $"/Hinh/KhachHang/{c.Hinh}",

                }).ToListAsync();

            // Xử lý logic tìm kiếm
            var results = productResults.Concat(cusResult).ToList();
            return View(results);
        }


        public IActionResult ExportDailyReport(DateTime date)
        {
            // Xác định khoảng thời gian trong ngày
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            // Lấy dữ liệu từ bảng HoaDons và ChiTietHoaDons
            var data = db.ChiTietHds.Include(p => p.MaHdNavigation).Include(p => p.MaHhNavigation).Where(p => p.MaHdNavigation.NgayDat.Date == DateTime.Now.Date)
                .Select(g => new DailyReportViewModel
				{
                    ProductName = g.MaHhNavigation.TenHh,
                    Quantity = g.SoLuong,
                    Price = g.DonGia,
                    Sale = g.GiamGia,
                    Revenue = (g.SoLuong * g.DonGia) * (1 - g.GiamGia)
                })
                .ToList();

            // Đếm tổng số hóa đơn trong ngày
            var totalInvoices = db.HoaDons
                .Count(hd => hd.NgayDat.Date == DateTime.Now.Date);

            // Gọi phương thức tạo file Excel
            return GenerateExcelReport(data, totalInvoices, startDate);
        }


        private IActionResult GenerateExcelReport(
            List<DailyReportViewModel> data,
            int totalInvoices,
            DateTime reportDate)
        {
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			// Khởi tạo EPPlus
			using (var package = new ExcelPackage())
            {
                // Tạo worksheet
                var worksheet = package.Workbook.Worksheets.Add("Daily Report");

                // Tiêu đề báo cáo
                worksheet.Cells[1, 1].Value = "Daily Report";
                worksheet.Cells[2, 1].Value = $"Ngày: {reportDate:yyyy-MM-dd}";
                worksheet.Cells[3, 1].Value = $"Tổng đơn hàng: {totalInvoices}";

                // Header
                worksheet.Cells[5, 1].Value = "Tên sản phẩm";
                worksheet.Cells[5, 2].Value = "Số lượng";
                worksheet.Cells[5, 3].Value = "Giá";
                worksheet.Cells[5, 4].Value = "Giảm giá";
                worksheet.Cells[5, 5].Value = "Doanh thu";

                // Thêm dữ liệu
                int row = 6;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.ProductName;
                    worksheet.Cells[row, 2].Value = item.Quantity;
                    worksheet.Cells[row, 3].Value = item.Price;
                    worksheet.Cells[row, 4].Value = item.Sale;
                    worksheet.Cells[row, 5].Value = item.Revenue;
                    row++;
                }

                // Định dạng bảng
                worksheet.Cells[5, 1, row - 1, 3].AutoFitColumns();
                worksheet.Cells[5, 1, row - 1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                // Xuất file Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"Daily_Report_{reportDate:yyyyMMdd}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }

        }
    }
}
