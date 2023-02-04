using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.Models
{
    public class Organize
    {
        public int ID { get; set; }

        [Display(Name ="Organized By")]
        public string OrganizedBy { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
