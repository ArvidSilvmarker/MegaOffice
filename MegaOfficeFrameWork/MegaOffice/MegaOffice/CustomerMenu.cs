using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOffice
{
    public class CustomerMenu
    {
        private DatabaseManager _db;
        private MainMenu _mainMenu;

        public CustomerMenu(DatabaseManager db, MainMenu mainMenu)
        {
            this._db = db;
            this._mainMenu = mainMenu;
        }

        public void Menu()
        {
            bool quit = false;
            while (!quit)
            {
                PrintCustomerMenu();
                var cmd = Console.ReadLine();
                Console.WriteLine();

                switch (cmd.ToUpper())
                {
                    case ("1"):
                        PrintCustomers(_db.ReadAllCustomers());
                        break;
                    case "2":
                        PrintSingleCustomer(_db.ReadCustomer(GetCustomerIDFromUser()));
                        break;
                    case "3":
                        _db.CreateCustomer(GetNewCustomerFromUser());
                        break;
                    case "4":
                        PrintCustomers(_db.ReadAllCustomers());
                        int id = GetCustomerIDFromUser();
                        _db.DeleteCustomer(id);
                        Console.WriteLine($"Tar bort kund {id}.");
                        Console.WriteLine();
                        break;
                    case "5":
                        PrintCustomers(_db.ReadAllCustomers());
                        _db.UpdateCustomer(GetUpdatedCustomerFromUser(_db));
                        Console.WriteLine();
                        break;
                    case "Q":
                    case "C":
                        quit = true;
                        break;


                }
            }
        }

        public void PrintCustomerMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Kundmeny");
            Console.WriteLine("(1) Läs in alla kunder.");
            Console.WriteLine("(2) Visa kund.");
            Console.WriteLine("(3) Lägg till ny kund.");
            Console.WriteLine("(4) Ta bort kund.");
            Console.WriteLine("(5) Ändra kund.");
            Console.WriteLine("(Q) Tillbaka till Huvudmenyn.");
            Console.Write("Kommando: ");
            Console.ResetColor();
        }

        public void PrintSingleCustomer(Customer customer)
        {
            WriteLineInColor($"{"Kundnummer",-12}{"Förnamn",-17}{"Efternamn",-22}{"E-postadress",-32}", ConsoleColor.Cyan);
            Console.WriteLine($"{customer.CustomerID,-12}{customer.FirstName,-17}{customer.LastName,-22}{customer.Email,-32}");
            Console.WriteLine();

            WriteLineInColor("Telefonnummer: ",ConsoleColor.Cyan);
            foreach (string phoneNr in customer.Phone ?? new List<string>())
            {
                Console.WriteLine($"{phoneNr}");
            }
            Console.WriteLine();

            WriteLineInColor("Intressanta produkter för kunden: ", ConsoleColor.Cyan);
            foreach (Product product in customer.InterestingProducts ?? new List<Product>())
            {
                WriteInColor($"Prodnr: ", ConsoleColor.DarkCyan);
                WriteLineInColor($"{product.ProductID,6}", ConsoleColor.Gray);
            }
            Console.WriteLine();

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

        public Customer GetUpdatedCustomerFromUser(DatabaseManager db)
        {
            int kundnummer = GetCustomerIDFromUser();
            Customer customer = db.ReadCustomer(kundnummer);
            WriteLineInColor("(1) Förnamn, (2) Efternamn, (3) Email, (4) Telefonnummer, (5) Intressanta produkter", ConsoleColor.Cyan);
            WriteInColor("Vad vill du ändra: ", ConsoleColor.DarkCyan);
            var answer = Console.ReadLine();
            switch (answer)
            {
                case "1":
                    customer.FirstName = UpdateValue();
                    break;
                case "2":
                    customer.LastName = UpdateValue();
                    break;
                case "3":
                    customer.Email = UpdateValue();
                    break;
                case "4":
                    ChangePhoneNumbers(customer);
                    break;
                case "5":
                    ChangeInterestingProducts(customer);
                    break;
            }

            return customer;
        }

        private void ChangeInterestingProducts(Customer customer)
        {
            if (customer.InterestingProducts.Count > 0)
            {
                WriteInColor("Ändra produkt - ", ConsoleColor.DarkCyan);
                for (int i = 0; i < customer.InterestingProducts.Count; i++)
                {
                    Console.WriteLine($"Ändra {customer.InterestingProducts[i].Name} ({i + 1})");
                }
                Console.WriteLine($"Lägg till ny produkt ({customer.Phone.Count + 1})");
                Console.Write("Val: ");
                int index = Convert.ToInt32(Console.ReadLine()) - 1;
                Product updatedProduct = _mainMenu.ProductMenu.GetProductFromUser();
                if (updatedProduct == null && index >= 0 && index < customer.InterestingProducts.Count)
                    customer.InterestingProducts.RemoveAt(index);
                else if (index >= 0 && index < customer.Phone.Count)
                    customer.InterestingProducts[index] = updatedProduct;
                else if (index == customer.InterestingProducts.Count)
                    customer.InterestingProducts.Add(updatedProduct);
            }
            else
            {
                customer.InterestingProducts.Add(_mainMenu.ProductMenu.EnterNewProduct());
            }

            Console.WriteLine();
        }

        public string UpdateValue()
        {
            Console.Write("Skriv in nytt värde: ");
            return Console.ReadLine();
        }

        private void ChangePhoneNumbers(Customer customer)
        {
            if (customer.Phone.Count > 0)
            {
                WriteInColor("Ändra telefonnummer - ", ConsoleColor.DarkCyan);
                for (int i = 0; i < customer.Phone.Count; i++)
                {
                    Console.WriteLine($"Ändra {customer.Phone[i]} ({i + 1})");
                }
                Console.WriteLine($"Lägg till nytt telefonnummer ({customer.Phone.Count + 1})");
                Console.Write("Val: ");
                int index = Convert.ToInt32(Console.ReadLine()) - 1;
                Console.Write("Skriv nytt telefonnummer (eller enter för inget telefonnummer): ");
                string input = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(input) && index >= 0 && index < customer.Phone.Count)
                    customer.Phone.RemoveAt(index);
                else if (index >= 0 && index < customer.Phone.Count)
                    customer.Phone[index] = input;
                else if (index == customer.Phone.Count)
                    customer.Phone.Add(input);
            }
            else
            {
                Console.Write("Skriv nytt telefonnummer: ");
                customer.Phone = new List<string> { Console.ReadLine() };
            }

            Console.WriteLine();
        }

        public int GetCustomerIDFromUser()
        {
            Console.Write("Skriv kundnummer: ");
            return Convert.ToInt32(Console.ReadLine());

        }

        public Customer GetNewCustomerFromUser()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Förnamn: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            var firstName = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Efternamn: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            var lastName = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Epost: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            var email = Console.ReadLine();

            List<string> phone = new List<string>();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Telefonummer (Tryck enter om du inte vill ange telefonnummer): ");
                Console.ForegroundColor = ConsoleColor.Gray;
                string input = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(input))
                    break;
                phone.Add(input);
            }


            Console.WriteLine();

            return new Customer(firstName, lastName, email, phone);
        }

        public void PrintCustomers(List<Customer> customerList)
        {
            //Stringalignment {blabla,-10} => ge blabla 10 teckens utrymme, och skriv från vänster.
            // {blabla,5} => ge blabla 5 teckens utrymme, och skriv från höger.

            WriteLineInColor($"{"Kundnummer",-12}{"Förnamn",-17}{"Efternamn",-22}{"E-postadress",-32}{"Telefonnummer",-20}{"Fler nr",-9}", ConsoleColor.Cyan);
            foreach (var customer in customerList)
            {
                Console.Write($"{customer.CustomerID,-12}{customer.FirstName,-17}{customer.LastName,-22}{customer.Email,-32}");
                Console.Write($"{customer.Phone.First(),-20}");
                if (customer.Phone.Where(phone => phone != null).ToList().Count > 1)
                    Console.Write($"{"    *",-9}");
                else
                    Console.Write($"{"",-9}");
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
