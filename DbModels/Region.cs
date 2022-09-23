using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Region
    {
        public Region()
        {
            Carriers = new HashSet<Carrier>();
            Stops = new HashSet<Stop>();
            Towns = new HashSet<Town>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdCou { get; set; }

        public virtual Country IdCouNavigation { get; set; }
        public virtual ICollection<Carrier> Carriers { get; set; }
        public virtual ICollection<Stop> Stops { get; set; }
        public virtual ICollection<Town> Towns { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
