using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class VehPaint
    {
        public int Id { get; set; }
        public string IdVeh { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Paint { get; set; }
        public string Note { get; set; }

        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
