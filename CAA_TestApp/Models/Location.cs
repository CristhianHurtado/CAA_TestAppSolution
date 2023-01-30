using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class Location
    {
        public int ID { get; set; }

        [Display(Name="Location")]
        [Required(ErrorMessage ="Location cannot be left blank.")]
        [StringLength(40, ErrorMessage ="Location cannot be longer than 40 characters.")]
        public string Name { get; set; }

        public ICollection<Inventory> Inventories { get; set; }
    }
}
