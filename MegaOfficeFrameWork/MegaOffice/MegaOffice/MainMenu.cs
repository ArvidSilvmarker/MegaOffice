﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOffice
{
    public class MainMenu
    {
        private DatabaseManager _db;
        public CustomerMenu CustomerMenu { get; set; }
        public ProductMenu ProductMenu { get; set; }

        public CategoryMenu CategoryMenu { get; set; }


        public MainMenu(bool useLocalDB)
        {
            if (useLocalDB)
                _db = new DatabaseManager();
            else
                _db = CreateAzureDBConnection();

            CustomerMenu = new CustomerMenu(_db, this);
            ProductMenu = new ProductMenu(_db, this);
            CategoryMenu = new CategoryMenu(_db, this);
        }

        private DatabaseManager CreateAzureDBConnection()
        {
            DatabaseManager db;

            while (true)
            {
                string username = GetUsernameFromUser();
                string password = GetPasswordFromUser();
                try
                {
                    db = new DatabaseManager(
                        $"Server = tcp:kaffedbserver.database.windows.net,1433; Initial Catalog = MegaOfficeDB; Persist Security Info = False; User ID ={username}; Password ={password}; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Authentication =\"Active Directory Password\";");
                }
                catch
                {
                    continue;
                }

                break;
            }

            return db;
        }

        private string GetUsernameFromUser()
        {
            Console.Write("Enter AzureAD Username: ");
            return Console.ReadLine();
        }

        public void Menu()
        {
            PrintWelcome();
            bool quit = false;
            while (!quit)
            {
                PrintMainMenu();
                var key = Console.ReadLine();
                Console.WriteLine();

                switch (key.ToUpper())
                {
                    case ("1"):
                        CustomerMenu.Menu();
                        break;
                    case "2":
                        ProductMenu.Menu();
                        break;
                    case "Q":
                    case "C":
                        quit = true;
                        break;
                }
            }
        }

        private void PrintWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Välkommen till MegaOffice!");
            Console.ResetColor();
        }

        public void PrintMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Huvudmeny");
            Console.WriteLine("(1) Kundmeny.");
            Console.WriteLine("(2) Produktmeny.");
            Console.WriteLine("(Q) Avsluta.");
            Console.Write("Kommando: ");
            Console.ResetColor();
        }

        public string GetPasswordFromUser()
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
    }
}
