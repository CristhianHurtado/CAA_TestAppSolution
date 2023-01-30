using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    //public interface Auditable
    //{
        public abstract class Auditable : IAuditable
        {
            [ScaffoldColumn(false)]
            [StringLength(256)]
            public string CreatedBy { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? CreatedOn { get; set; }

            [ScaffoldColumn(false)]
            [StringLength(256)]
            public string UpdatedBy { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? UpdatedOn { get; set; }

            [ScaffoldColumn(false)]
            public string OrderedBy { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? OrderedOn { get; set; }
            
            [ScaffoldColumn(false)]
            public string TookBy { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? TookOn { get; set; }

            [ScaffoldColumn(false)]
            public string ReturnedBy { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? ReturnedOn { get; set; }

            [ScaffoldColumn(false)]
            public string LocationChangedBy { get; set; }

            [ScaffoldColumn(false)]
            public string LastLocation { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? LocationchangedOn { get; set; }

            [ScaffoldColumn(false)]
            public string ShelfMoveBy { get; set; }

            [ScaffoldColumn(false)]
            public DateTime? ShelfMoveOn { get; }

            [ScaffoldColumn(false)]
            [Timestamp]
            public Byte[] RowVersion { get; set; }//Added for concurrency

        }
    //}
}
