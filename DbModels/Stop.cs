using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Stop
    {
        public Stop()
        {
            Photos = new HashSet<Photo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? IdStr { get; set; }
        public int IdReg { get; set; }

        public virtual Region IdRegNavigation { get; set; }
        public virtual Street IdStrNavigation { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
