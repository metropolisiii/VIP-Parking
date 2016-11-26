using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIP_Parking.ViewModels
{
    public class AllowedLotsVM
    {
        public int Lot_ID { get; set; }
        public string Lot_Name { get;set; }
        public bool Allowed { get; set; }
    }
}