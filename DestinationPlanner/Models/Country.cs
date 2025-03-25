using System.ComponentModel.DataAnnotations;

namespace DestinationPlanner.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        public string CountryName { get; set; }

        public ICollection<City> Cities { get; set; } = new HashSet<City>();
        public ICollection<Destination> Destinations { get; set; } = new HashSet<Destination>();
    }
}
