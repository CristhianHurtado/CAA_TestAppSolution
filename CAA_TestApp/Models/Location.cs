using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace CAA_TestApp.Models
{
    public class Location : Auditable
    {
        public int ID { get; set; }

        [Display(Name="Location")]
        [Required(ErrorMessage ="Location cannot be left blank.")]
        [StringLength(40, ErrorMessage ="Location cannot be longer than 40 characters.")]
        public string City { get; set; }

        [Display(Name="Phone")]
        [Required(ErrorMessage ="Phone number cannot be left blank.")]
        [RegularExpression("^\\d{10}$", ErrorMessage ="Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Display(Name ="Address")]
        [Required(ErrorMessage ="Address cannot be left blank.")]
        [StringLength(100, ErrorMessage ="Address cannot be longer than 100 characters.")]
        public string Address { get; set; }

        [Display(Name ="Postal Code")]
        [Required(ErrorMessage ="Postal Code cannot be left blank. Please enter a Postal Code (without spaces).")]
        //[RegularExpression("^\\[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$", ErrorMessage ="Please enter a valid 6 character postal code (without spaces).")]
        [DataType(DataType.PostalCode)]
        [StringLength(6)]
        public string PostalCode { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency


        [Display(Name ="Phone")]
        public string PhoneFormatted
        {
            get
            {
                return "(" + Phone.Substring(0,3) + ") " + Phone.Substring(3,3) + "-" + Phone[6..];
            }
        }

        public ICollection<Inventory> Inventories { get; set; }
    }
}
