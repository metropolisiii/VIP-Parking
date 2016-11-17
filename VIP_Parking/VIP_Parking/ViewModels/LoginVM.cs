using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIP_Parking.ViewModels
{
    public class LoginVM
    {
        [Required, AllowHtml]
        public string Username { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}