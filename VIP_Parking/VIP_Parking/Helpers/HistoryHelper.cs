using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;

namespace VIP_Parking.Helpers
{
    public static class HistoryHelper
    {
        public static void AddToHistory(string action, int reservationID)
        {
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
            History history = new History();
            history.Date = DateTime.Now;
            history.Action = action;
            history.Reserv_ID = reservationID;
            db.Histories.Add(history);
            db.SaveChanges();
            return;
        }
    }
}