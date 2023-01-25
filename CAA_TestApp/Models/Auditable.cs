using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class Auditable : IAuditable
    {
        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string OrderedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? OrderedOn { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string TookBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? TookOn { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string ReturnedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? ReturnedOn { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string ShelfMoveBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? ShelfMoveOn { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string LocationChangedBy { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string LastLocation { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? LocationchangedOn { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency
    }
}
