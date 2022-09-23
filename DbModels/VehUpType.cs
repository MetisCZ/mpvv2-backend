using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class VehUpType
    {
        public VehUpType()
        {
            VehTypeHistories = new HashSet<VehTypeHistory>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdVet { get; set; }

        public virtual VehType IdVetNavigation { get; set; }
        public virtual ICollection<VehTypeHistory> VehTypeHistories { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
