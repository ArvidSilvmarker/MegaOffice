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
            Console.Write("Ladda lokal databas? (ja/nej): ");
            bool localDB = Console.ReadLine().ToLower().Trim() == "ja" ? true : false;
            var program = new MainMenu(true);
            program.Menu();
        }
    }
}
