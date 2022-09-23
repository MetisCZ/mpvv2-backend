using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class VehUrl
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string IdVeh { get; set; }
        public int Position { get; set; }

        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
