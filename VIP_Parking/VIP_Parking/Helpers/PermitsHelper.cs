using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;
using VIP_Parking.ViewModels;

namespace VIP_Parking.Helpers
{
    public static class PermitsHelper
    {
        public static List<string> CreatePermits(ReservationVM reservationVM)
        {
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
            List<string> attachments = new List<string>();

            for (int i = 0; i < reservationVM.NumOfSlots; i++)
            {
                //Get the last permit code so that we can add one to it
                int permit_number = 1000;
                var permit_number_query = db.Permits.OrderByDescending(p => p.PermitCode).FirstOrDefault();
                if (permit_number_query != null)
                    permit_number = (int)permit_number_query.PermitCode + 1;
                Permit permit = new Permit { PermitCode = permit_number, Reserv_ID = reservationVM.Reserv_ID };
                db.Permits.Add(permit);
                db.SaveChanges();
                QRCodeHelper.GenerateRelayQrCode(permit_number);
                attachments.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files/") + "qrcode_" + permit_number + ".gif");
            }
            return attachments;
        }
    }
}