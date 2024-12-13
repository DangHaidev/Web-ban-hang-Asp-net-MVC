using System.ComponentModel.DataAnnotations;

namespace Web_ban_hang.ViewModels
{
	public class LoginVM
	{
		[Display(Name = "Tên đăng nhập")]
		[Required(ErrorMessage = "Chưa nhập tên đăng nhập")]
		[MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
		public string UserName { get; set; }

		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Chưa nhập mật khẩu")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}

	public class ForgotPass
	{
		[Required]
		public string Email { get; set; }
		
		public string Code { get; set; }
	}
	public class rePass
	{
		[Required]
		public string Pass { get; set; }
	}
}
