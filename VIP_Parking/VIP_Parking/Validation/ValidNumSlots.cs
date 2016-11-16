using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VIP_Parking.Models.Database;
using System.Linq;
using VIP_Parking.ViewModels;

namespace VIPParking.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidNumSlots : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string start_temp, end_temp;
            if (value != null)
            {
                 VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
                //Select the number slots iavailable for the lot and get the number of slots already taken. If the value of spaces is greater this, throw a validation exception.
                var slotsInLot = db.Lots.Where(s => s.Lot_Name == "VIP").FirstOrDefault();
                var model = (ReservationVM)validationContext.ObjectInstance;
                start_temp = model.Date.ToString("yyyy-MM-dd") + " " + model.Start_Time + " " + model.Start_Ampm;
                end_temp = model.Date.ToString("yyyy-MM-dd") + " " + model.End_Time + " " + model.End_Ampm;

                DateTime end_time = DateTime.ParseExact(end_temp, "yyyy-MM-dd h:mm tt", null);
                DateTime start_time = DateTime.ParseExact(start_temp, "yyyy-MM-dd h:mm tt", null);
                var slotsTaken = (from s in db.Reservations where s.Approved == true && ((s.Start_Time <= start_time && s.End_Time > start_time) || (s.Start_Time < end_time && s.End_Time >= end_time) || (s.Start_Time >= start_time && s.End_Time < end_time )) select (int?)s.NumOfSlots).Sum() ?? 0;
                if (Convert.ToInt32(value) > slotsInLot.Lot_Spaces_Available - slotsTaken)
                {
                    return new ValidationResult("You have exceeded the number of available spaces. The number of avaiable spaces is: " + (slotsInLot.Lot_Spaces_Available - slotsTaken)+". You may check back later to see if any spaces have opened up. <button id='waiting_list' name='waiting_list' value='waiting_list' type='submit'>Place me on a waiting list</button> ");
                }
            }
            return ValidationResult.Success;
        }
    }
}