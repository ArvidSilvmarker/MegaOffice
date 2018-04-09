using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;

namespace MegaOffice
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DatabaseManager("Server=tcp:kaffedbserver.database.windows.net,1433;Initial Catalog=MegaOfficeDB;Persist Security Info=False;User ID=ServerAdmin;Password=MandelbiskviHallongrotta2;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");



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
                        db.UpdateCustomer(GetUpdatedCustomerFromUser(db));
                        Console.WriteLine();
                        break;
                    case "Q":
                    case "C":
                        quit = true;
                        break;


                }
            }


            
            

        }

        private static Customer GetUpdatedCustomerFromUser(DatabaseManager db)
        {
            int kundnummer = GetCustomerIDFromUser();
            Customer customer = db.ReadCustomer(kundnummer);
            Console.WriteLine("(1) Förnamn, (2) Efternamn, (3) Email, (4) Telefonnummer");
            Console.Write("Vad vill du ändra: ");
            var answer = Console.ReadLine();
            Console.Write("Skriv in nytt värde: ");
            var value = Console.ReadLine();
            
            switch (answer)
            {
                case "1":
                    customer.FirstName = value;
                    break;
                case "2":
                    customer.LastName = value;
                    break;
                case "3":
                    customer.Email = value;
                    break;
                case "4":
                    customer.Phone = value;
                    break;
            }

            return customer;
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
            Console.WriteLine("(2) Lägg till ny kund.");
            Console.WriteLine("(3) Ta bort kund.");
            Console.WriteLine("(4) Ändra kund.");
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
