//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class korisnik
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public korisnik()
        {
            this.sluzbe1 = new HashSet<sluzbe>();
        }
    
        public int ID { get; set; }
        public int ID_sluzbe { get; set; }
        public string Naziv { get; set; }
        public Nullable<int> canedit { get; set; }
        public Nullable<int> cannew { get; set; }
        public Nullable<int> caninsertfile { get; set; }
        public Nullable<int> canexportexcel { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sluzbe> sluzbe1 { get; set; }
        public virtual sluzbe sluzbe { get; set; }
    }
}
