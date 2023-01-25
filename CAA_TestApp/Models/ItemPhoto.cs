using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class ItemPhoto
    {
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        [StringLength(255)]
        public string MimeType { get; set; }

        public int invID { get; set; }
        public Inventory inventory { get; set; }
    }
}
