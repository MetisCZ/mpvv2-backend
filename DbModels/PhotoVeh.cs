using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class PhotoVeh
    {
        public int IdPho { get; set; }
        public string IdVeh { get; set; }

        public virtual Photo IdPhoNavigation { get; set; }
        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
