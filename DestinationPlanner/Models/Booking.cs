using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DestinationPlanner.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [ForeignKey("Package")]
        [DisplayName("Package")]
        public int PackageId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? TravelDate { get; set; }
        public Package? Package { get; set; }
    }
}
