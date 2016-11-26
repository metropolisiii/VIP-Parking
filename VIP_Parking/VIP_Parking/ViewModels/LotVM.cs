using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIP_Parking.ViewModels
{
    public class LotVM
    {
        public int Lot_ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}