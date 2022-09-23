using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class CarrierNameHistory
    {
        public int Id { get; set; }
        public int IdCar { get; set; }
        public string Name { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public virtual Carrier IdCarNavigation { get; set; }
    }
}
