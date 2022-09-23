using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Carrier
    {
        public Carrier()
        {
            CarrierNameHistories = new HashSet<CarrierNameHistory>();
            CarrierUrls = new HashSet<CarrierUrl>();
            Depots = new HashSet<Depot>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdReg { get; set; }
        public DateTime LastUpdate { get; set; }

        public virtual Region IdRegNavigation { get; set; }
        public virtual ICollection<CarrierNameHistory> CarrierNameHistories { get; set; }
        public virtual ICollection<CarrierUrl> CarrierUrls { get; set; }
        public virtual ICollection<Depot> Depots { get; set; }
    }
}
