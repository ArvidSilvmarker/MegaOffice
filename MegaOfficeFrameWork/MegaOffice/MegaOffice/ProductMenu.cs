using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOffice
{
    public class ProductMenu
    {
        private DatabaseManager _db;
        private MainMenu _mainMenu;

        public ProductMenu(DatabaseManager db, MainMenu mainMenu)
        {
            this._db = db;
            this._mainMenu = mainMenu;
        }

        public void Menu()
        {
            bool quit = false;
            while (!quit)
            {
                PrintProductMenu();
                var cmd = Console.ReadLine();
                Console.WriteLine();

                switch (cmd.ToUpper())
                {
                    case ("1"):
                        PrintProducts(_db.ReadAllProducts());
                        break;
                    case "2":
                        PrintSingleProduct(_db.ReadProduct(GetProductIDFromUser()));
                        break;
                    case "3":
                        _db.CreateProduct(GetNewProductFromUser());
                        break;
                    case "4":
                        PrintProducts(_db.ReadAllProducts());
                        _db.UpdateProduct(GetUpdatedProductFromUser());
                        break;
                    case "5":
                        PrintProducts(_db.ReadAllProducts());
                        int id = GetProductIDFromUser();
                        _db.DeleteProduct(id);
                        WriteLineInColor($"Tar bort produkt {id}.", ConsoleColor.Red);
                        Console.WriteLine();
                        break;
                    case "Q":
                    case "C":
                        quit = true;
                        break;
                }
            }
        }

        public void PrintProductMenu()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kundmeny");
            Console.WriteLine("(1) Visa alla produkter.");
            Console.WriteLine("(2) Visa produkt.");
            Console.WriteLine("(3) Lägg till ny produkt.");
            Console.WriteLine("(4) Ändra produkt.");
            Console.WriteLine("(5) Ta bort produkt.");
            Console.WriteLine("(Q) Tillbaka till Huvudmenyn.");
            Console.Write("Kommando: ");
            Console.ResetColor();
        }

        public Product GetProductFromUser()
        {
            PrintProducts(_db.ReadAllProducts());
            int productID = GetProductIDFromUser();
            return _db.ReadProduct(productID);
        }

        private int GetProductIDFromUser()
        {
            Console.Write("Skriv produktnummer: ");
            return Convert.ToInt32(Console.ReadLine());
        }

        public void PrintProducts(List<Product> productList)
        {
            WriteInColor($"{"Produktnummer",-15}{"Namn",-22}{"Pris",10}{"Kategori",-22}{"Intressant för",-30}", ConsoleColor.Red);
            foreach (var product in productList)
            {
                Console.WriteLine($"{product.ProductID,-15}{product.Name,-22}{product.Price,10:##.00}{product.Category.Name,-22}");
            }

            Console.WriteLine();
        }

        public void PrintSingleProduct(Product product)
        {
            WriteInColor($"{"Produktnummer",-15}{"Namn",-22}{"Pris",10}{"Kategori",-22}{"Intressant för",-30}", ConsoleColor.Red);
            Console.Write($"{product.ProductID,-15}{product.Name,-22}{product.Price,10:##.00}{product.Category.Name,-22}");
            Console.WriteLine(CustomerIDString(product.InterestedCustomers));
            Console.WriteLine();

        }

        private string CustomerIDString(List<Customer> customers)
        {
            string text = "";
            foreach (Customer customer in customers)
            {
                text += $"{customer.CustomerID} ";
            }

            return text;
        }

        public void WriteInColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public void WriteLineInColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public Product EnterNewProduct()
        {
            WriteInColor("Produktnamn: ",ConsoleColor.DarkMagenta);
            string name = Console.ReadLine();
            WriteInColor("Pris: ",ConsoleColor.DarkMagenta);
            Decimal price = Convert.ToDecimal(Console.ReadLine());
            Category category = GetCategoryFromUser();
            return new Product
            {
                Name = name,
                Price = price,
                Category = category,
                InterestedCustomers = new List<Customer>()
            };
        }





    }
}
