using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class VehCarierList
    {
        public int Id { get; set; }
        public string IdVeh { get; set; }
        public int IdCar { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string DateFrom2 { get; set; }
        public string DateTo2 { get; set; }
        public int State { get; set; }

        public virtual DepotForCarrierList IdCarNavigation { get; set; }
        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
