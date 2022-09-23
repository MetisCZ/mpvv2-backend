using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class DepartOdis
    {
        public long Id { get; set; }
        public string IdVeh { get; set; }
        public string IdVeh2 { get; set; }
        public string IdVeh3 { get; set; }
        public DateTime ActDate { get; set; }
        public string Line { get; set; }
        public int Route { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public int Delay { get; set; }
        public int? StartStation { get; set; }
        public int? FinalStation { get; set; }
        public int? LastStation { get; set; }
        public bool WasIn { get; set; }

        public virtual Vehicle IdVeh2Navigation { get; set; }
        public virtual Vehicle IdVeh3Navigation { get; set; }
        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
