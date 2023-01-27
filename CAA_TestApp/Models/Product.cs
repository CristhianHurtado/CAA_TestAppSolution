using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CAA_TestApp.Models
{
    public class Product : Auditable
    {
        public int ID { get; set; }

        [Display(Name = "Item")]
        public string Name { get; set; }

        public int CategoryID { get; set; }

        public Category Category { get; set; }

        public ICollection<Inventory> Inventories { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
