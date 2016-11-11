using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VIP_Parking.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        public int Dept_ID { get; set; }
        public string Dept_name { get; set; }
        public virtual ICollection<Requester> Requesters { get; set; }
    }
}