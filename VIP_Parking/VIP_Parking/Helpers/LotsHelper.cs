using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;
using VIP_Parking.ViewModels;

namespace VIP_Parking.Helpers
{
    public static class LotsHelper
    {
        public static List<AllowedLotsVM> PopulateAllowedLotsData(Reservation reservation)
        {
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
            var allLots = db.Lots;
            var reservationLots = new HashSet<int>(reservation.Lots.Select(l => l.Lot_ID));
            var viewModel = new List<AllowedLotsVM>();
            foreach (var lot in allLots)
            {
                viewModel.Add(new AllowedLotsVM
                {
                    Lot_ID = lot.Lot_ID,
                    Lot_Name = lot.Lot_Name,
                    Allowed = reservationLots.Contains(lot.Lot_ID)
                });
            }
            return viewModel;
        }
    }
}