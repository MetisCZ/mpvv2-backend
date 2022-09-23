using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class TypeDetail
    {
        public int Id { get; set; }
        public string TKey { get; set; }
        public string TValue { get; set; }
        public string IdVeh { get; set; }
        public int Position { get; set; }

        public virtual Vehicle IdVehNavigation { get; set; }
    }
}
