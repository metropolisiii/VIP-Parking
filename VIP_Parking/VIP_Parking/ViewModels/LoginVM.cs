using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VIP_Parking.ViewModels
{
    public class LoginVM
    {
        [Required, AllowHtml]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsLocked { get; set; }
    }
}