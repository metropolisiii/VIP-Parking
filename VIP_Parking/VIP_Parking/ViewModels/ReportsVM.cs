using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VIP_Parking.ViewModels
{
    [VIPParking.ValidationAttributes.ReportsValidator]
    public class ReportsVM
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string EndDate { get; set; }
        public Nullable<int> Category_ID { get; set; }
        public Nullable<int> Dept_ID { get; set; }
        public string DeptName { get; set; }
        public int DeptCount { get; set; }
        public string CategoryName { get; set; }
        public int CategoryCount { get; set; }
    }
}