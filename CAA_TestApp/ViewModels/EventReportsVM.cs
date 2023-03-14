using System.ComponentModel.DataAnnotations;

namespace CAA_TestApp.ViewModels
{
    public class EventReportsVM
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name of the event cannot be left blank.")]
        [StringLength(50, ErrorMessage = "Event name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Quantity cannot be left blank.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Event date cannot be left blank.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Event location cannot be left blank.")]
        [StringLength(40, ErrorMessage = "Event location cannot be longer than 40 characters.")]
        public string EventLocation { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}
