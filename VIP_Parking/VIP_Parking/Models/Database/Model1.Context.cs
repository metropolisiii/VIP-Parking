﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VIPPARKINGEntities1 : DbContext
    {
        public VIPPARKINGEntities1()
            : base("name=VIPPARKINGEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<GateCode> GateCodes { get; set; }
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<ParkingSpot> ParkingSpots { get; set; }
        public virtual DbSet<Permit> Permits { get; set; }
        public virtual DbSet<Requester> Requesters { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
    }
}
