using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CAA_TestApp.Models
{
    public class Product :Auditable
    {
        public int ID { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Product name cannot be left blank.")]
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency


        [Display(Name="Category")]
        [Required(ErrorMessage ="Select the category for this product.")]
        public int CategoryID { get; set; }

        public Category Category { get; set; }

        public ICollection<Inventory> Inventories { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
