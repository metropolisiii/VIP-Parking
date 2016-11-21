using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;
using VIP_Parking.ViewModels;

namespace VIP_Parking.Helpers
{
    public static class NumSlotsHelper
    {
        public static int getSlotsTaken(DateTime start_time, DateTime end_time)
        {
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
            var slotsTaken = (from s in db.Reservations where s.Approved == 1 && ((s.Start_Time <= start_time && s.End_Time > start_time) || (s.Start_Time < end_time && s.End_Time >= end_time) || (s.Start_Time >= start_time && s.End_Time < end_time)) select (int?)s.NumOfSlots).Sum() ?? 0;
            return slotsTaken;
        }
        public static int getSlotsInLot()
        {
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
            var slotsInLot = db.Lots.Where(s => s.Lot_Name == "VIP").FirstOrDefault();
            return slotsInLot.Lot_Spaces_Available;
        }
        public static bool isReserveable(ReservationVM reservation)
        {
            string start_temp, end_temp;
            
            //Select the number slots iavailable for the lot and get the number of slots already taken. If the value of spaces is greater this, throw a validation exception.
           
            var model = reservation;
            start_temp = model.Date + " " + model.Start_Time + " " + model.Start_Ampm;
            end_temp = model.Date + " " + model.End_Time + " " + model.End_Ampm;

            DateTime end_time = DateTime.ParseExact(end_temp, "MM/dd/yyyy h:mm tt", null);
            DateTime start_time = DateTime.ParseExact(start_temp, "MM/dd/yyyy h:mm tt", null);
            
            if (Convert.ToInt32(reservation.NumOfSlots) > getSlotsInLot() - getSlotsTaken(start_time, end_time))
            {
                return false;
            }
            return true;
        }          
    }
}