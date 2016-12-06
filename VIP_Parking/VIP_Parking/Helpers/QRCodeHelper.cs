using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIP_Parking.Models.Database;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;

namespace VIP_Parking.Helpers
{
    public static class QRCodeHelper
    {
        public static void GenerateRelayQrCode(int permitID)
        {
            var qrValue = ConfigurationManager.AppSettings["host"].ToString()+"/Reservations/Search?SearchString="+permitID.ToString();
            QRCodeWriter qr = new QRCodeWriter();
            var temp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files/");
            temp = temp + "qrcode_" + permitID+".gif";
            var matrix = qr.encode(qrValue, ZXing.BarcodeFormat.QR_CODE, 200, 350);
            ZXing.BarcodeWriter w = new ZXing.BarcodeWriter();
            w.Format = ZXing.BarcodeFormat.QR_CODE;
            w.Renderer = new BitmapRenderer();
            Bitmap img = w.Write(matrix);

            //Add permit text
            using (var g = Graphics.FromImage(img))
            using (var font = new Font(FontFamily.GenericMonospace, 12))
            using (var brush = new SolidBrush(Color.Black))
            using (var format = new StringFormat() { Alignment = StringAlignment.Center })
            {
                int margin = 5, textHeight = 20;

                var rect = new RectangleF(margin, img.Height - (2*textHeight),
                                          img.Width - 2 * margin, textHeight);

                //Get permit info
                VIPPARKINGEntities1 db = new VIPPARKINGEntities1();
                Permit permit = db.Permits.Find(permitID);
                if (permit == null)
                {
                    return;
                }
                string inputData = permit.Reservation.Start_Time.ToString("MM/dd");
                g.DrawString(inputData, font, brush, rect, format);
                rect.Y = img.Height - textHeight;
                inputData = permit.Reservation.Start_Time.ToString("h:mmtt") + "-" + permit.Reservation.End_Time.ToString("h:mmtt");
                g.DrawString(inputData, font, brush, rect, format);
                rect.Y = 0;
                g.DrawString("Regis University", font, brush, rect, format);
                rect.Y = textHeight;
                g.DrawString("VIP Parking Permit", font, brush, rect, format);
                rect.Y = textHeight * 2;
                g.DrawString(permit.Reservation.RecipientName, font, brush, rect, format);
            }
            img.Save(temp, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}