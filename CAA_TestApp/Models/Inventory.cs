using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CAA_TestApp.Models
{
    public class Inventory : Auditable
    {
        public int ID { get; set; }
        
        public string ISBN { get; set; }

               [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion{ get; set; }//Added for concurrency

        [Display(Name = "Quantity")]  
        public int Quantity { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Shelf")]
        public string ShelfOn { get; set; }

        public double Cost { get; set; }

        public DateTime DateReceived { get; set; }

        [Display(Name = "Location")]
        public int LocationID { get; set; }

        public int ProductID { get; set; }

        public Location Location { get; set; }

        public Product Product { get; set; }

        public ItemPhoto ItemPhoto { get; set; }

        public ItemThumbnail ItemThumbnail { get; set; }
        
        public QrImage QRImage { get; set; }
        

    }
}
