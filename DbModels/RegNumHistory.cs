using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class RegNumHistory
    {
        public int Id { get; set; }
        public string IdVeh { get; set; }
        public string Name { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
