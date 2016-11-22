using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VIP_Parking.Models.Database;

namespace VIP_Parking.ViewModels
{
    public class ReservationVM
    {
        public int Reserv_ID { get; set; }
        [Required(ErrorMessage = "Guest Name is required")]
        public string RecipientName { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string RecipientEmail { get; set; }
        public Nullable<int> Category_ID { get; set; }
        public Nullable<int> Event_ID { get; set; }
        public string Event { get; set; }
        [Required]
        [VIPParking.ValidationAttributes.CheckDateRange]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string Date { get; set; }
        [Required(ErrorMessage = "From Time is required")]
        [RegularExpression("^(1[0-2]|[1-9]):[0-5][0-9]", ErrorMessage = "Invalid time format (h:mm)")]
        public string Start_Time { get; set; }
        public string Start_Ampm { get; set; }
        [Required(ErrorMessage = "End Time is required")]
        [RegularExpression("^(1[0-2]|[1-9]):[0-5][0-9]", ErrorMessage = "Invalid time format (h:mm)")]
        public string End_Time { get; set; }
        public string End_Ampm { get; set; }
        [Required(ErrorMessage = "At least 1 space is needed")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Value not within a valid range")]
        public int NumOfSlots { get; set; }
        public int Requester_ID { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Requester_Email { get; set; }
        public Nullable<int> Dept_ID { get; set; }
        public int GateCode { get; set; }
        public byte Approved { get; set; }
    }
}