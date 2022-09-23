using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Street
    {
        public Street()
        {
            Photos = new HashSet<Photo>();
            Stops = new HashSet<Stop>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? IdTow { get; set; }

        public virtual Town IdTowNavigation { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<Stop> Stops { get; set; }
    }
}
