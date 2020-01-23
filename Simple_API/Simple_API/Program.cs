using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net;

namespace Simple_API
{
    class Program
    {
        //server and client
        private static TcpClient client;
        private static TcpListener server;

        //global networkstream for recieving data
        private static NetworkStream stream;

        //global buffer
        private static byte[] buffer = new byte[1024];

        

        static void Main(string[] args)
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


        //handle multible clients
        private static void StartLestining()
        {
            try
            {
                //accept clients and recieve requests from them
                server.BeginAcceptTcpClient(new AsyncCallback(Handle), server);
            }
            catch(EntryPointNotFoundException ex)
            {
                Console.WriteLine("warning: " + ex.Message);
            }
        }

        private static void Handle(IAsyncResult res)
        {
            try
            {
                //reaccept clients because the next statement will end accepting
                StartLestining();                
                TcpClient cli = server.EndAcceptTcpClient(res);


                //add client to our list
                Client c = new Client(cli);
                Client.clients.Add(c);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

     

       

    }
}
