using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            VehTypes = new HashSet<VehType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<VehType> VehTypes { get; set; }
    }
}
