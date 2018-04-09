using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace MegaOffice
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DatabaseManager();



            bool quit = false;
            while (!quit)
            {
                PrintMenu();
                var cmd = Console.ReadLine();
                Console.WriteLine();

                switch (cmd.ToUpper())
                {
                    case ("1"):
                        PrintCustomers(db.ReadAllCustomers());
                        break;
                    case "2":
                        db.CreateCustomer(GetNewCustomerFromUser());
                        break;
                    case "3":
                        PrintCustomers(db.ReadAllCustomers());
                        int id = GetCustomerIDFromUser();
                        db.DeleteCustomer(id);
                        Console.WriteLine("Tar bort kund {id}.");
                        Console.WriteLine();
                        break;
                    case "4":
                        PrintCustomers(db.ReadAllCustomers());
                        db.UpdateCustomer(GetUpdatedCustomerFromUser());
                        Console.WriteLine();
                        break;   
                    case "Q":
                    case "C":
                        quit = true;
                        break;


                }
            }


            
            

        }

        private static Customer GetUpdatedCustomerFromUser()
        {
            int kundnummer = GetCustomerIDFromUser();
            return new Customer("Förnamn", "Efternamn", "Email", "Telefonnummer");
        }

        private static int GetCustomerIDFromUser()
        {
            Console.Write("Skriv kundnummer: ");
            return Convert.ToInt32(Console.ReadLine());

        }

        private static Customer GetNewCustomerFromUser()
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

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Telefonummer: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            var phone = Console.ReadLine();

            Console.WriteLine();

            return new Customer(firstName, lastName, email, phone);
        }

        static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Välkommen till MegaOffice");
            Console.WriteLine("(1) Läs in alla kunder.");
            Console.WriteLine("(2) Ny kund.");
            Console.WriteLine("(3) Ta bort kund. ");
            Console.WriteLine("(Q) Avsluta.");
            Console.Write("Kommando: ");
            Console.ResetColor();
        }

        static void PrintCustomers(List<Customer> customerList)
        {
            //Stringalignment {blabla,-10} => ge blabla 10 teckens utrymme, och skriv från vänster.
            // {blabla,5} => ge blabla 5 teckens utrymme, och skriv från höger.

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{"Kundnummer",-15}{"Förnamn",-20}{"Efternamn",-20}{"E-postadress",-40}{"Telefonnummer",-20}");
            Console.ResetColor();
            foreach (var customer in customerList)
            {
                Console.WriteLine($"{customer.Kundnummer,-15}{customer.FirstName,-20}{customer.LastName,-20}{customer.Email,-40}{customer.Phone,-20}");
            }

            Console.WriteLine();
        }

    }
}
