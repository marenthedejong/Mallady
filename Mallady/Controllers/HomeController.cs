using Mallady.Database;
using Mallady.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

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
                                id = Convert.ToInt32(reader["Id"]),
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

        [Route("Betaalpagina")]
        public IActionResult Betaalpagina()
        {
            return View();
        }

        [Route("Product/{id}")]
        public IActionResult Product(string id)
        {
            var model = GetProduct(id);

            ViewData["product"] = GetProduct(id);

            return View(model);
        }

        private Product GetProduct(string id)
        {
            
            List<Product> product = new List<Product>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"select * from product where id = {id}", conn);
                using (var reader = cmd.ExecuteReader()) 
                {
                    while (reader.Read())
                    {
                        Product p = new Product
                        {
                            // selecteer de kolommen die je wil lezen. In dit geval kiezen we de kolom "naam"
                            id = Convert.ToInt32(reader["Id"]),
                            Naam = reader["naam"].ToString(),
                            Ingredienten = reader["ingredienten"].ToString(),
                            Prijs = Convert.ToInt32(reader["prijs"]),
                            Voorraad = Convert.ToInt32(reader["voorraad"]),
                            Img = reader["foto"].ToString(),
                        };

                        // voeg de naam toe aan de lijst met namen
                        product.Add(p);
                    }
                }

            }

            return product[0];
        }


        [Route("Vestigingen")]
        public IActionResult Vestigingen()
        {
            var restaurant = GetRestaurants();
            return View(restaurant);
        }

        [HttpPost]
        [Route("Contact")]
        public IActionResult Contact(Person person)
        {
            if (ModelState.IsValid)
            {
                SavePerson(person);
                return Redirect("/Succes");
            }
          
           return View(person);
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("Succes")]
        public IActionResult Succes()
        {
            return View();
        }

        [Route("notfound")]
        public IActionResult notfound()
        {
            return View();
        }


        private void SavePerson(Person person)
        {
            person.Wachtwoord = ComputeSha256Hash(person.Wachtwoord);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO klant(voornaam, achternaam, gebruikersnaam, wachtwoord, email, bericht) VALUES(?voornaam, ?achternaam, ?gebruikersnaam, ?wachtwoord, ?email, ?bericht)", conn);
                
                cmd.Parameters.Add("?voornaam", MySqlDbType.Text).Value = person.Voornaam;
                cmd.Parameters.Add("?achternaam", MySqlDbType.Text).Value = person.Achternaam;
                cmd.Parameters.Add("?gebruikersnaam", MySqlDbType.Text).Value = person.Gebruikersnaam;
                cmd.Parameters.Add("?wachtwoord", MySqlDbType.Text).Value = person.Wachtwoord;
                cmd.Parameters.Add("?email", MySqlDbType.Text).Value = person.Email;
                cmd.Parameters.Add("?bericht", MySqlDbType.Text).Value = person.Bericht;
                cmd.ExecuteNonQuery();
            }
        }

        [Route("Reserveren")]
        public IActionResult Reserveren()
        {
            return View();
        }

        [HttpPost]
        [Route("Reserveren")]
        public IActionResult Reserveren(Reservering reservering)
        {
            if (ModelState.IsValid)
            {
                SaveReservering(reservering);
                return Redirect("/Succestwo");
            }

            return View(reservering);
        }

        [Route("Succestwo")]
        public IActionResult Succestwo()
        {
            return View();
        }

        private void SaveReservering(Reservering reservering)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO reservering(voornaam, achternaam, restaurant_id, personen, datumtijd, locatie) VALUES(?voornaam, ?achternaam, ?restaurant_id, ?personen, ?datumtijd, ?locatie)", conn);

                cmd.Parameters.Add("?voornaam", MySqlDbType.Text).Value = reservering.Voornaam;
                cmd.Parameters.Add("?achternaam", MySqlDbType.Text).Value = reservering.Achternaam;
                cmd.Parameters.Add("?restaurant_id", MySqlDbType.Int32).Value = reservering.Restaurant_id;
                cmd.Parameters.Add("?datumtijd", MySqlDbType.DateTime).Value = reservering.Datumtijd;
                cmd.Parameters.Add("?personen", MySqlDbType.Int32).Value = reservering.Personen;
                cmd.Parameters.Add("?locatie", MySqlDbType.Text).Value = reservering.Locatie;
                cmd.ExecuteNonQuery();
            }
        }

        public List<Restaurant> GetRestaurants()
        {

            // maak een lege lijst waar we de namen in gaan opslaan
            List<Restaurant> Restaurants = new List<Restaurant>();

            // verbinding maken met de database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // verbinding openen
                conn.Open();

                // SQL query die we willen uitvoeren
                MySqlCommand cmd = new MySqlCommand("select * from restaurant", conn);

                // resultaat van de query lezen
                using (var reader = cmd.ExecuteReader())
                {
                    // elke keer een regel (of eigenlijk: database rij) lezen
                    while (reader.Read())
                    {
                        Restaurant r = new Restaurant

                        {
                            id = Convert.ToInt32(reader["Id"]),
                            Locatie = reader["locatie"].ToString(),
                            Adres = reader["adres"].ToString(),
                            Image = reader["fotovestiging"].ToString(),

                        };

                        Restaurants.Add(r);
                    }
                }
            }

            // return de lijst met namen
           return Restaurants;
        }


        [Route("Login")]
        public IActionResult Login(string gebruikersnaam, string wachtwoord)
        {
            if (!string.IsNullOrWhiteSpace(wachtwoord))
            {
                Person p = GetPerson(gebruikersnaam);
                if (p == null)
                    return View();

                string hash = ComputeSha256Hash(wachtwoord);

                if (p.Wachtwoord == hash)
                {
                    HttpContext.Session.SetString("User", gebruikersnaam);
                    return Redirect("/Ingelogd");
                }
            }

            return View();
        }

        [Route("Ingelogd")]
        public IActionResult Ingelogd()
        {
            ViewData["user"] = HttpContext.Session.GetString("User");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private Person GetPerson(string gebruikersnaam)
        {
            List<Person> persons = new List<Person>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"select * from klant where gebruikersnaam = '{gebruikersnaam}'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Person p = new Person
                        {
                            // selecteer de kolommen die je wil lezen. In dit geval kiezen we de kolom "naam"
                            Wachtwoord = reader["Wachtwoord"].ToString(),

                        };

                        persons.Add(p);
                    }
                }
            }
            return persons[0];
        }


    }

}