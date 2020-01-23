using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write(@"

             _   _                _ _ 
            | \ | |              | (_)
            |  \| | ___ _ __ ___ | |_ 
            | . ` |/ _ \ '__/ _ \| | |
            | |\  |  __/ | | (_) | | |
            |_| \_|\___|_|  \___/|_|_|
                                      
                                      

");
            Server.Run();
            Console.ReadLine();
        }
    }
}
