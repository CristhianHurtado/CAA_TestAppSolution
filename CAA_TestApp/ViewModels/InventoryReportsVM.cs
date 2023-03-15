using CAA_TestApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace CAA_TestApp.ViewModels
{
    public class InventoryReportsVM
    {
        public int ID { get; set; }

        [Display(Name = "Product")]
        public string Product { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Organized By")]
        [Required(ErrorMessage = "Organized By field cannot be left blank.")]
        public string OrganizeBy { get; set; }

        [Required(ErrorMessage = "You must enter a cost for the inventory total.")]
        [DataType(DataType.Currency)]
        public double Cost { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Select the location for this inventory record.")]
        public int LocationID { get; set; }


        [Display(Name = "Location")]
        public string Location { get; set; }

    }
}
