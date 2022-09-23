using System;
using System.Collections.Generic;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class Photo
    {
        public Photo()
        {
            PhotoVehs = new HashSet<PhotoVeh>();
        }

        public int Id { get; set; }
        public int? IdSto { get; set; }
        public int? IdStr { get; set; }
        public DateTime PhotoDate { get; set; }
        public DateTime UploadDate { get; set; }
        public string Note { get; set; }
        public string Url { get; set; }

        public virtual Stop IdStoNavigation { get; set; }
        public virtual Street IdStrNavigation { get; set; }
        public virtual ICollection<PhotoVeh> PhotoVehs { get; set; }
    }
}
