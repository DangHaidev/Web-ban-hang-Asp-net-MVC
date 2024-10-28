using System.Text;

namespace Web_ban_hang.Helpers
{
	public class Util
	{

		public static string UploadHinh(IFormFile Hinh,string folder)
		{

			try
			{
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
				using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
				{
					Hinh.CopyTo(myfile);
				}
				return Hinh.FileName;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}

		}

		public static string GenerateRamdomKey(int length = 5)
		{
			var pattern = @"btttjafkwbskdjjsbdfsDNHD!@#";
			var sb = new StringBuilder();
			var rd = new Random(length);
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}
			return sb.ToString();	
		}
	}
}
