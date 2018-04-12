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
            var program = new MainMenu();
            program.Menu();
        }
    }
}
