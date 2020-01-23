using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Server
{
    public abstract class Alerts
    {
        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!]ERROR: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+]DONE: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void INFO(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[*]INFO: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[-]WARNING: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Debug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[+]Debug: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
