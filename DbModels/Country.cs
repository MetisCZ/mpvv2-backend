using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Country
    {
        public Country()
        {
            Regions = new HashSet<Region>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Flag { get; set; }
        public string Abbreviation { get; set; }

        public virtual ICollection<Region> Regions { get; set; }
    }
}
