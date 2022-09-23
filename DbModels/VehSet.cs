using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class VehSet
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public string IdVeh1 { get; set; }
        public string IdVeh2 { get; set; }
        public string IdVeh3 { get; set; }

        public virtual Vehicle IdVeh1Navigation { get; set; }
        public virtual Vehicle IdVeh2Navigation { get; set; }
        public virtual Vehicle IdVeh3Navigation { get; set; }
    }
}
