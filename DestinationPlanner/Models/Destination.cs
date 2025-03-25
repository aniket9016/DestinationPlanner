using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DestinationPlanner.Models
{
    public class Destination
    {
        [Key]
        public int DestinationId { get; set; }

        [Required]
        [DisplayName("Destination")]
        public string DestinationName { get; set; }

        [ForeignKey("Country")]
        [DisplayName("Country")]
        public int CountryId { get; set; }

        [ForeignKey("City")]
        [DisplayName("City")]
        public int CityId { get; set; }

        public Country? Country { get; set; }
        public City? City { get; set; }
        public ICollection<Package>? Packages { get; set; }
    }
}
