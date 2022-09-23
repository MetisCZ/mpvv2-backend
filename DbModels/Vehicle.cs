using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            DepartOdisIdVeh2Navigations = new HashSet<DepartOdis>();
            DepartOdisIdVeh3Navigations = new HashSet<DepartOdis>();
            DepartOdisIdVehNavigations = new HashSet<DepartOdis>();
            PhotoVehs = new HashSet<PhotoVeh>();
            RegNumHistories = new HashSet<RegNumHistory>();
            TypeDetails = new HashSet<TypeDetail>();
            VehCarierLists = new HashSet<VehCarierList>();
            VehNotes = new HashSet<VehNote>();
            VehPaints = new HashSet<VehPaint>();
            VehSetHistoryIdVeh1Navigations = new HashSet<VehSetHistory>();
            VehSetHistoryIdVeh2Navigations = new HashSet<VehSetHistory>();
            VehSetHistoryIdVeh3Navigations = new HashSet<VehSetHistory>();
            VehSetIdVeh2Navigations = new HashSet<VehSet>();
            VehSetIdVeh3Navigations = new HashSet<VehSet>();
            VehTypeHistories = new HashSet<VehTypeHistory>();
            VehUrls = new HashSet<VehUrl>();
        }

        public string Id { get; set; }
        public int? IdDep { get; set; }
        public int? IdVut { get; set; }
        public int IdReg { get; set; }
        public string LongRegNum { get; set; }
        public string RegNum { get; set; }
        public string ManufacYear { get; set; }
        public int AirCondition { get; set; }
        public sbyte Part { get; set; }
        public int Cond { get; set; }
        public DateTime AddDate { get; set; }
        public string MiniPicture { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime? LastSeen { get; set; }
        public int Departs { get; set; }
        public bool Listable { get; set; }
        public string Spz { get; set; }
        public int? SerialNumber { get; set; }
        public string Vin { get; set; }
        public int LowFloor { get; set; }
        public string InfoPanel { get; set; }
        public int WasInCount { get; set; }
        public DateTime? WasInLast { get; set; }

        public virtual Depot IdDepNavigation { get; set; }
        public virtual Region IdRegNavigation { get; set; }
        public virtual VehUpType IdVutNavigation { get; set; }
        public virtual VehSet VehSetIdVeh1Navigation { get; set; }
        public virtual ICollection<DepartOdis> DepartOdisIdVeh2Navigations { get; set; }
        public virtual ICollection<DepartOdis> DepartOdisIdVeh3Navigations { get; set; }
        public virtual ICollection<DepartOdis> DepartOdisIdVehNavigations { get; set; }
        public virtual ICollection<PhotoVeh> PhotoVehs { get; set; }
        public virtual ICollection<RegNumHistory> RegNumHistories { get; set; }
        public virtual ICollection<TypeDetail> TypeDetails { get; set; }
        public virtual ICollection<VehCarierList> VehCarierLists { get; set; }
        public virtual ICollection<VehNote> VehNotes { get; set; }
        public virtual ICollection<VehPaint> VehPaints { get; set; }
        public virtual ICollection<VehSetHistory> VehSetHistoryIdVeh1Navigations { get; set; }
        public virtual ICollection<VehSetHistory> VehSetHistoryIdVeh2Navigations { get; set; }
        public virtual ICollection<VehSetHistory> VehSetHistoryIdVeh3Navigations { get; set; }
        public virtual ICollection<VehSet> VehSetIdVeh2Navigations { get; set; }
        public virtual ICollection<VehSet> VehSetIdVeh3Navigations { get; set; }
        public virtual ICollection<VehTypeHistory> VehTypeHistories { get; set; }
        public virtual ICollection<VehUrl> VehUrls { get; set; }
    }
}
