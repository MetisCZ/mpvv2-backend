using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Town
    {
        public Town()
        {
            Streets = new HashSet<Street>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdReg { get; set; }

        public virtual Region IdRegNavigation { get; set; }
        public virtual ICollection<Street> Streets { get; set; }
    }
}
