using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        
        public string ISBN { get; set; }

        [Display(Name = "Item")]
        public string Name { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Shelf")]
        public string ShelfOn { get; set; }

        public double Cost { get; set; }

        public DateTime DateReceived { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Location")]
        public int LocationID { get; set; }

        [Display(Name = "Category")]
        public Category Category { get; set; }

        [Display(Name = "Location")]
        public Location Location { get; set; }

        public ItemPhoto ItemPhoto { get; set; }

        public ItemThumbnail ItemThumbnail { get; set; }

    }
}
