using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VIP_Parking.Models
{
    [Table("Requester")]
    public class Requester
    {
        [Key]
        public int Requester_ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Dept_ID { get; set; }
        public string Email { get; set; }

        public virtual Department Department { get; set; }
    }
}