using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class VehType
    {
        public VehType()
        {
            VehUpTypes = new HashSet<VehUpType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdMan { get; set; }
        public int Traction { get; set; }

        public virtual Manufacturer IdManNavigation { get; set; }
        public virtual ICollection<VehUpType> VehUpTypes { get; set; }
    }
}
