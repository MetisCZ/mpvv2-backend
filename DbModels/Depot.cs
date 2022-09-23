using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Depot
    {
        public Depot()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdCar { get; set; }

        public virtual Carrier IdCarNavigation { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
