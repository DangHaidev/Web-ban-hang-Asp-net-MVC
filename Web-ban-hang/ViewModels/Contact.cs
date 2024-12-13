using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Web_ban_hang.Models.Entities;

namespace Web_ban_hang.ViewModels
{
    public class Contact
    {
        [Required]
        public string HoTen { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string NoiDung { get; set; }
        public DateTime NgayGy { get; set; }

        public int? MaCd { get; set; }      
        public bool? CanTraLoi { get; set; }
        public string? MaGy { get; set; }
        
    }


}
