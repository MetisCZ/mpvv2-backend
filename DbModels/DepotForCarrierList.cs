using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class DepotForCarrierList
    {
        public DepotForCarrierList()
        {
            VehCarierLists = new HashSet<VehCarierList>();
        }

        public int Id { get; set; }
        public int IdVcl { get; set; }
        public int IdDep { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public virtual ICollection<VehCarierList> VehCarierLists { get; set; }
    }
}
