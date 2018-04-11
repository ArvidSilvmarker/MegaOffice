using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOffice
{
    public static class Menu
    {
        public static void PrintSingleCustomer(Customer customer)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{"Kundnummer",-15}{"Förnamn",-20}{"Efternamn",-20}{"E-postadress",-40}{"Telefonnummer",-20}");
            Console.ResetColor();
            if (customer != null)
            {

                Console.Write($"{customer.Kundnummer,-15}{customer.FirstName,-20}{customer.LastName,-20}{customer.Email,-40}");
                foreach (string phoneNr in customer.Phone)
                {
                    Console.WriteLine($"{phoneNr,-20}");
                    Console.Write($"{"",-95}");
                }

            }

            Console.WriteLine();
        }

        public static string GetPasswordFromUser()
        {
            Console.Clear();
            string pass = "";
            Console.Write("Enter your password: ");
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace resets entry
                if (key.Key != ConsoleKey.Backspace)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    Console.Clear();
                    pass = "";
                    Console.Write("Enter your password: ");
                    continue;
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.Clear();
            return pass;
        }

        public static Customer GetUpdatedCustomerFromUser(DatabaseManager db)
        {
            int kundnummer = GetCustomerIDFromUser();
            Customer customer = db.ReadCustomer(kundnummer);
            Console.WriteLine("(1) Förnamn, (2) Efternamn, (3) Email, (4) Telefonnummer");
            Console.Write("Vad vill du ändra: ");
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
                    customer.Phone = ChangePhoneNumbers(customer);
                    break;
            }

            return customer;
        }

        public static string UpdateValue()
        {
            Console.Write("Skriv in nytt värde: ");
            return Console.ReadLine();
        }

        private static List<string> ChangePhoneNumbers(Customer customer)
        {
            if (customer.Phone.Count > 0)
            {
                Console.WriteLine("Ändra telefonnummer - ");
                for (int i = 0; i < customer.Phone.Count; i++)
                {
                    Console.WriteLine($"Ändra {customer.Phone[i]} ({i + 1})");
                }
                Console.WriteLine($"Lägg till nytt telefonnummer ({customer.Phone.Count+1})");
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
                customer.Phone = new List<string> {Console.ReadLine()};
            }

            Console.WriteLine();
            return customer.Phone;
        }

        public static int GetCustomerIDFromUser()
        {
            Console.Write("Skriv kundnummer: ");
            return Convert.ToInt32(Console.ReadLine());

        }

        public static Customer GetNewCustomerFromUser()
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

        public static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Välkommen till MegaOffice");
            Console.WriteLine("(1) Läs in alla kunder.");
            Console.WriteLine("(2) Visa kund.");
            Console.WriteLine("(3) Lägg till ny kund.");
            Console.WriteLine("(4) Ta bort kund.");
            Console.WriteLine("(5) Ändra kund.");
            Console.WriteLine("(Q) Avsluta.");
            Console.Write("Kommando: ");
            Console.ResetColor();
        }

        public static void PrintCustomers(List<Customer> customerList)
        {
            //Stringalignment {blabla,-10} => ge blabla 10 teckens utrymme, och skriv från vänster.
            // {blabla,5} => ge blabla 5 teckens utrymme, och skriv från höger.

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{"Kundnummer",-15}{"Förnamn",-20}{"Efternamn",-20}{"E-postadress",-40}{"Telefonnummer (* = flera nummer)",-20}");
            Console.ResetColor();
            foreach (var customer in customerList)
            {
                if (customer.Phone.Where(phone => phone != null).ToList().Count > 1)
                    Console.WriteLine($"{customer.Kundnummer,-15}{customer.FirstName,-20}{customer.LastName,-20}{customer.Email,-40}{customer.Phone.First(),-20} *");
                else
                    Console.WriteLine($"{customer.Kundnummer,-15}{customer.FirstName,-20}{customer.LastName,-20}{customer.Email,-40}{customer.Phone.First(),-20}");
            }

            Console.WriteLine();
        }
    }
}
