using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;

namespace VIP_Parking.Helpers
{
    public static class EventsHelper
    {
        public static int GetOrCreateEvent(string eventname, DateTime date)
        {
            int eventID;
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
            var e = db.Events.Where(i => i.Event_Name.ToLower() == eventname.ToLower());
            if (e.Count() == 0)
            {
                var eventrec = new VIP_Parking.Models.Database.Event
                {
                    Event_Name = eventname,
                    Event_Start_Time = date,
                    Event_End_Time = date.AddHours(23),
                    Event_Spots_Needed = 0
                };
                db.Events.Add(eventrec);
                db.SaveChanges();
                eventID = eventrec.Event_ID;
            }
            else
                eventID = e.FirstOrDefault().Event_ID;
            return eventID;
        }
    }
}