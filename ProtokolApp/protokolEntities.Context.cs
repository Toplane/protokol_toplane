﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProtokolApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class protokolEntities1 : DbContext
    {
        public protokolEntities1()
            : base("name=protokolEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<dokument> dokument { get; set; }
        public virtual DbSet<korisnik> korisnik { get; set; }
        public virtual DbSet<protokol> protokol { get; set; }
        public virtual DbSet<sluzbe> sluzbe { get; set; }
        public virtual DbSet<tip> tip { get; set; }
        public virtual DbSet<instanca> instanca { get; set; }
        public virtual DbSet<promjene> promjene { get; set; }
    }
}
