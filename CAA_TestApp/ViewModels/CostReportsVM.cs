using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CAA_TestApp.ViewModels
{
    public class CostReportsVM
    {
        public int ID { get; set; }

        [Display(Name = "Product")]
        public string Product { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "You must enter a cost for the inventory total.")]
        [DataType(DataType.Currency)]
        public double Cost { get; set; }

        [DataType(DataType.Currency)]
        public double CostTotal { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Select the location for this inventory record.")]
        public int LocationID { get; set; }
    }
}
