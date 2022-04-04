using System.ComponentModel.DataAnnotations;

namespace Mallady.Models
{
    public class Reservering
    {
        public string? Voornaam { get; set; }

        public string? Achternaam { get; set; }

        public string? Restaurant_id { get; set; }

        public int? Personen { get; set; }

        public int? Datum { get; set; }

        public int? Tijd { get; set; }

    }

}

 