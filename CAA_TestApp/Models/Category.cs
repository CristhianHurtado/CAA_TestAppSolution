using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class Category
    {
        public int ID { get; set; }

        [Display(Name="Category")]
        [Required(ErrorMessage ="Category cannot be left blank.")]
        [StringLength(20, ErrorMessage ="Category cannot be longer than 20 characters.")]
        public string Classification { get; set; }

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();

        //public ICollection<Inventory> Inventories { get; set; }

        //public Category()
        //{
        //    this.Inventories = new HashSet<Inventory>();
        //}
    }
}
