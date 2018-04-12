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
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                    case "7":
                        break;
                    case "8":
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
            Console.WriteLine("(2) Visa alla kategorier.");
            Console.WriteLine("(3) Visa produkt.");
            Console.WriteLine("(4) Visa kategori.");
            Console.WriteLine("(5) Ändra produkt.");
            Console.WriteLine("(6) Ändra kategori.");
            Console.WriteLine("(7) Ta bort produkt.");
            Console.WriteLine("(8) Ta bort kategori.");
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

        private Category GetCategoryFromUser()
        {
            PrintCategories(_db.ReadAllCategories());
            WriteInColor("Välj kategori: ", ConsoleColor.DarkMagenta);
            return _db.ReadCategory(Convert.ToInt32(Console.ReadLine())-1);
        }

        private void PrintCategories(List<Category> categoryList)
        {
            WriteInColor($"{"Kategorinummer",-15}{"Namn",-22}", ConsoleColor.Red);
            foreach (var category in categoryList)
            {
                Console.WriteLine($"{category.CategoryID,-15}{category.Name,-22}");
            }

            Console.WriteLine();
        }

        public Category EnterNewCategory()
        {
            return new Category();
        
        }

    }
}
