using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Procrastaway
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Start the console app. Does not return. */
            procrastawayConsole procManager = new procrastawayConsole();
            try
            {
                procManager.Start();
            }
            finally
            {
                procManager.stop();
            }
            
        }
    }
}
