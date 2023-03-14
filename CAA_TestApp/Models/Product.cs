using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace CAA_TestApp.Models
{
    public class Product : Auditable
    {
        public int ID { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Product name cannot be left blank.")]
        [StringLength(100, ErrorMessage ="Product name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Display(Name="Par Level")]
        [Required(ErrorMessage ="Par Level cannot be left blank.")]
        public int ParLevel { get; set; }

        [Display(Name="Category")]
        [Required(ErrorMessage ="Select the category for this product.")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Organized By")]
        [Required(ErrorMessage = "You must select how you will be tracking the count for this product.")]
        public int OrganizeID { get; set; }
        public Organize Organize { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency

        [JsonIgnore]
        public ICollection<Inventory> Inventories { get; set; } = new HashSet<Inventory>();

    }
}
