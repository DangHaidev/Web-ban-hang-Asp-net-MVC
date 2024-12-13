using AutoMapper;
using Web_ban_hang.Models.Entities;
using Web_ban_hang.ViewModels;

namespace Web_ban_hang.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile() {
			CreateMap<RegisterVM, KhachHang>();
			CreateMap<KhachHang, Customer>();
		}

	}
}
