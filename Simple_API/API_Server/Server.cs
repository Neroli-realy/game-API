using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace API_Server
{
     class Server
    {
        //server and client
        private static TcpClient client;
        private static TcpListener server;

        //global networkstream for recieving data
        private static NetworkStream stream;

        //global buffer
        private static byte[] buffer = new byte[1024];

        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[+]Starting server...");
            Console.ForegroundColor = ConsoleColor.White;

            //initialize server
            server = new TcpListener(IPAddress.Any, 8686);
            server.Start();

            try
            {

                StartLestining();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+]Listening....");
                Console.WriteLine("[+]Press Anykey to exit");
                Console.ForegroundColor = ConsoleColor.White;

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!]ERROR: " + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
            }
            Console.ReadLine();
        }
        public static void StartLestining()
        {
            try
            {
                //accept clients and recieve requests from them
                server.BeginAcceptTcpClient(new AsyncCallback(Handle), server);
            }
            catch (EntryPointNotFoundException ex)
            {
                Console.WriteLine("warning: " + ex.Message);
            }
        }

        public static void Handle(IAsyncResult res)
        {
            try
            {
                StartLestining();
                TcpClient client = server.EndAcceptTcpClient(res);

                Player player = new Player(client);
                Player.Connected.Add(player);
            }
            catch(Exception ex)
            {
                Alerts.Error(ex.Message);
            }
        }

    }
}
