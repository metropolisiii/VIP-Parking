//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VIP_Parking.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reservation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reservation()
        {
            this.Permits = new HashSet<Permit>();
        }
    
        public int Reserv_ID { get; set; }
        public int Requester_ID { get; set; }
        public string RecipientName { get; set; }
        public Nullable<int> NumOfSlots { get; set; }
        public string RecipientEmail { get; set; }
        public Nullable<int> Category_ID { get; set; }
        public int ParkingSpotID { get; set; }
        public Nullable<int> Event_ID { get; set; }
        public Nullable<int> GateCode { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Event Event { get; set; }
        public virtual GateCode GateCode1 { get; set; }
        public virtual ParkingSpot ParkingSpot { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permit> Permits { get; set; }
        public virtual Requester Requester { get; set; }
    }
}
