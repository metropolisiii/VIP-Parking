//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VIP_Parking.Models.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reservation
    {
        public int Reserv_ID { get; set; }
        public int User_ID { get; set; }
        public int Dept_ID { get; set; }
        public int Category_ID { get; set; }
        public int ParkingSpotID { get; set; }
        public int Event_ID { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Department Department { get; set; }
        public virtual Event Event { get; set; }
        public virtual ParkingSpot ParkingSpot { get; set; }
        public virtual User User { get; set; }
    }
}
