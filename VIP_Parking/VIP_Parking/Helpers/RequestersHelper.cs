using System;
using System.Linq;
using VIP_Parking.Models.Database;

namespace VIP_Parking.Helpers
{
    public static class RequestersHelper
    {
        private static VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
        public static int Upsert(string username, string firstname, string lastname, string email, int deptID)
        {
            var requester_results = db.Requesters.Where(u => u.Username == username).Single();
            //If record doesn't exist, insert it
            if (requester_results == null)
            {
                var requester = new VIP_Parking.Models.Database.Requester
                {
                    Username = username,
                    Firstname = firstname,
                    Lastname = lastname,
                    Dept_ID = deptID,
                    Email = email,
                    Fullname = firstname + " " + lastname,
                    IsLocked = false
                };
                db.Requesters.Add(requester);
                db.SaveChanges();
            }
            else //If record already exists, update it
            {
                requester_results.Firstname = firstname;
                requester_results.Lastname = lastname;
                requester_results.Dept_ID = deptID;
                requester_results.Email = email;
                requester_results.Fullname = firstname + " " + lastname;
                try
                {
                   db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return requester_results.Requester_ID;            
        }
        public static bool IsAdmin(int requesterID)
        {
            var requester_results = db.Requesters.Where(r => r.Requester_ID == requesterID).Single();
            return requester_results.IsAdmin;
        }
        public static bool IsLocked(string username)
        {
            using (VIPPARKINGEntities1 db = new VIPPARKINGEntities1())
            {
                var user = db.Requesters.Where(r => r.Username == username).Single();
                if (user == null)
                    return false;
                return user.IsLocked;
            }
        }
    }
}