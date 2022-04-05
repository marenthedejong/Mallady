using System.ComponentModel.DataAnnotations;

namespace Mallady.Models
{
    public class Reservering
    {
        [Required(ErrorMessage = "U bent verplicht uw voornaam in te vullen")]
        public string? Voornaam { get; set; }

        [Required(ErrorMessage = "U bent verplicht uw achternaam in te vullen")]
        public string? Achternaam { get; set; }

        [Required(ErrorMessage = "U bent verplicht een restaurant aan te geven")]
        public int? Restaurant_id { get; set; }

        [Required(ErrorMessage = "U bent verplicht het aantal personen op te geven")]
        public int? Personen { get; set; }

        [Required(ErrorMessage = "U bent verplicht een datum op te geven")]
        public Date? Datum { get; set; }

        [Required(ErrorMessage = "U bent verplicht een tijd op te geven")]
        public Time? Tijd { get; set; }

    }

}

 