using Mallady.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Mallady.Database;

namespace Mallady.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string connectionString = "Server=informatica.st-maartenscollege.nl;Port=3306;Database=110374;Uid=110374;Pwd=inf2021sql;";
        //private readonly string connectionString = "Server=172.16.160.21;Port=3306;Database=110374;Uid=110374;Pwd=inf2021sql;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        public List<Product> GetProducts()
        {
           
            // maak een lege lijst waar we de namen in gaan opslaan
            List<Product> products = new List<Product>();

            // verbinding maken met de database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // verbinding openen
                conn.Open();

                // SQL query die we willen uitvoeren
                MySqlCommand cmd = new MySqlCommand("select * from product", conn);

                // resultaat van de query lezen
                using (var reader = cmd.ExecuteReader())
                {
                    // elke keer een regel (of eigenlijk: database rij) lezen
                    while (reader.Read())
                    { 
                        Product p = new Product
                        {
                            // selecteer de kolommen die je wil lezen. In dit geval kiezen we de kolom "naam"
                            Id = Convert.ToInt32(reader["Id"]),
                            Naam = reader["naam"].ToString(),
                            Ingredienten = reader["ingredienten"].ToString(),
                            Prijs = Convert.ToInt32(reader["prijs"]),
                            Img = reader["foto"].ToString(),
                        };

                    // voeg de naam toe aan de lijst met namen
                    products.Add(p);
                }
            }
         }

         // return de lijst met namen
         return products;
        }


        [Route("Bestelpagina")]
        public IActionResult Bestelpagina()
        {
            var product = GetProducts();
            return View(product);
        }

        [Route("Menu")]
        public IActionResult Menu()
        {
            return View();
        }

        [Route("Vestigingen")]
        public IActionResult Vestigingen()
        {
            return View();
        }

        [HttpPost]
        [Route("Contact")]
        public IActionResult Contact(Person person)
        {
            if (ModelState.IsValid)
            {
                SavePerson(person);
                return Redirect("/Success");
            }
            return View(person);
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("Success")]
        public IActionResult Success()
        {
            return View();
        }

        private void SavePerson(Person person)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO klant(voornaam, achternaam, wachtwoord, email, telefoon, rekeningnummer, bericht) VALUES(?voornaam, ?achternaam, ?wachtwoord, ?telefoon, ?rekeningnummer, ?email, ?bericht)", conn);
                cmd.Parameters.Add("?voornaam", MySqlDbType.Text).Value = person.Voornaam;
                cmd.Parameters.Add("?achternaam", MySqlDbType.Text).Value = person.Achternaam;
                cmd.Parameters.Add("?wachtwoord", MySqlDbType.Text).Value = person.Wachtwoord;
                cmd.Parameters.Add("?email", MySqlDbType.Text).Value = person.Email;
                cmd.Parameters.Add("?telefoon", MySqlDbType.Int32).Value = person.Telefoon;
                cmd.Parameters.Add("?rekeningnummer", MySqlDbType.Int32).Value = person.Rekeningnummer;
                cmd.Parameters.Add("?bericht", MySqlDbType.Text).Value = person.Bericht;
                cmd.ExecuteNonQuery();
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}