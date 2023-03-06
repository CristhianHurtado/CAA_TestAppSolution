using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class EventInventory
    {
        [Display(Name ="Logged Out")]
        public int AmountLoggedOut { get; set; }

        [Display(Name ="Returned")]
        public int AmountReturned { get; set; }

        public int InventoryID { get; set; }
        public Inventory Inventory { get; set; }

        public int EventID { get; set; }
        public Event Event { get; set; }

    }
}
