using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class CarrierUrl
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int IdCar { get; set; }
        public int Position { get; set; }
        public string Note { get; set; }

        public virtual Carrier IdCarNavigation { get; set; }
    }
}
