using DestinationPlanner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DestinationPlanner.Models
{
    public class Package
    {
        [Key]
        public int PackageId { get; set; }
        [Required]
        public string PackageName { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Duration { get; set; }
        [Required]
        [ForeignKey("Destination")]
        public int DestinationId { get; set; }
        public Destination? Destination { get; set; }
        public ICollection<Booking>? Bookings{ get; set; }

    }
}
