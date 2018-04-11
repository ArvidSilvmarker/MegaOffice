using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;

namespace MegaOffice
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager db;
            while (true)
            {
                string password = Menu.GetPasswordFromUser();
                try
                {
                    db = new DatabaseManager(
                        $"Server = tcp:kaffedbserver.database.windows.net,1433; Initial Catalog = MegaOfficeDB; Persist Security Info = False; User ID = arvid.silvmarker@collectorbank.se; Password ={password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Authentication =\"Active Directory Password\";");
                }
                catch
                {
                    continue;
                }

                break;
            }


            bool quit = false;
            while (!quit)
            {
                Menu.PrintMenu();
                var cmd = Console.ReadLine();
                Console.WriteLine();

                switch (cmd.ToUpper())
                {
                    case ("1"):
                        Menu.PrintCustomers(db.ReadAllCustomers());
                        break;
                    case "2":
                        Menu.PrintSingleCustomer(db.ReadCustomer(Menu.GetCustomerIDFromUser()));
                        break;
                    case "3":
                        db.CreateCustomer(Menu.GetNewCustomerFromUser());
                        break;
                    case "4":
                        Menu.PrintCustomers(db.ReadAllCustomers());
                        int id = Menu.GetCustomerIDFromUser();
                        db.DeleteCustomer(id);
                        Console.WriteLine($"Tar bort kund {id}.");
                        Console.WriteLine();
                        break;
                    case "5":
                        Menu.PrintCustomers(db.ReadAllCustomers());
                        db.UpdateCustomer(Menu.GetUpdatedCustomerFromUser(db));
                        Console.WriteLine();
                        break;
                    case "Q":
                    case "C":
                        quit = true;
                        break;


                }
            }





        }
    }
}
