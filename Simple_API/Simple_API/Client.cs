using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Simple_API
{
   public class Client
    {
        private TcpClient client;
        public static List<Client> clients = new List<Client>();

        public string name;
        private string password;
        private bool error;
        private byte[] buffer;
        private NetworkStream stream;
        private Player p;
        public bool authorized = false;
        public Client(TcpClient client)
        {
           
                this.client = client;
                handle();
            
        }

        private void handle()
        {
            if (client.Connected)
            {
                stream = this.client.GetStream();
                buffer = new byte[1024];
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReaCB), stream);
            }
        }
        public Client() { }
        private void ReaCB(IAsyncResult res)
        {
            stream = (NetworkStream)res.AsyncState;

            var hh = stream.EndRead(res);
            string data = Encoding.UTF8.GetString(buffer, 0, hh);
            if (data.StartsWith("LOGIN:"))
            {
                this.name = data.Split(':')[1];
                this.password = data.Split(':')[2];
                if (Login())
                {
                    string response = "INFO:";
                    string[] info = this.getInfo();
                    foreach(string s in info)
                        response += s + ":";
                    send(response);
                    p = new Player(this.name);
                }

            }
            else if (data.StartsWith("REG:"))
            {
                this.name = data.Split(':')[1];
                this.password = data.Split(':')[2];
                Register();
            }
            else if (data.StartsWith("LOGOUT"))
            {
                
            }
            else if (data.StartsWith("GETUSERS"))
            {
                List<Player> players = new List<Player>();
                players = Mysqlcommands.get(players, this.name, true);
                string response = "PLAYERS:";
                foreach (Player p in players)
                {
                    response += p.name + ":";
                }
                send(response);
            }
            else if (data.StartsWith("ADD:"))
            {
                //p.sendAdd(new Player(data.Split(':')[1]));
            }
            else if (data.StartsWith("EXIT"))
                Exit();
            else
                Error("Bad Request!");
            handle();
        }
        private bool isExist()
        {
            //connection to mysql server
            string connection = "datasource=localhost;port=3306;username=root;password=root";
            MySqlConnection sqlc = new MySqlConnection(connection);

            //query to excute using parameterized query to avoid sqlinjection
            string query = "select  * from data.users where Name=@uname;";

            MySqlCommand sqlcom = new MySqlCommand(query, sqlc);
            sqlcom.Parameters.AddWithValue("@uname", this.name);

          
            //to read data from the database
            MySqlDataReader reader;
            try
            {
                //checks if the record is exist or no 
                sqlc.Open();
                reader = sqlcom.ExecuteReader();
               // Console.WriteLine(sqlcom.CommandText);
                int i = 0;
                while (reader.Read())  i++;
                if (i > 0) return true;

            }
            catch 
            {
                //handle connection error 
                Console.WriteLine("Error with Connection");
                Error("Request failed please try again later");
                error = true;
                return false;
            }

            return false;

        }

        private bool Register()
        {
            if (!this.isExist())
            {
                string connection = "datasource=localhost;port=3306;username=root;password=root";
                MySqlConnection sqlc = new MySqlConnection(connection);

                //query to excute
                string query = "insert into Data.users(Name, Password) values(@uname, @upass)";
                MySqlCommand sqlcom = new MySqlCommand(query, sqlc);
                sqlcom.Parameters.AddWithValue("@uname", this.name);
                sqlcom.Parameters.AddWithValue("@upass", this.password);


                //to read data from the database
                MySqlDataReader reader;
                try
                {
                    //checks if the record is exist or no 
                    sqlc.Open();
                    reader = sqlcom.ExecuteReader();
                    sqlc.Close();
                    try
                    {
                        query = "insert into data.playersdata(Name) values(@uname)";
                        sqlcom = new MySqlCommand(query, sqlc);
                        sqlcom.Parameters.AddWithValue("@uname", this.name);
                        sqlc.Open();
                        reader = sqlcom.ExecuteReader();
                        send("success:Regestration Done Sucssesfully!");
                        Console.WriteLine("User " + this.name + " Registrated");
                        sqlc.Close();
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                  
                }
                catch (MySqlException ex)
                {
                    //handle connection error 
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[!]ERROR: " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    Error("Request failed please try again later");
                    error = true;
                }
                return false;

            }
            else
            {
                Error("this user already exist");
                return false;
            }
        }

        private bool Login()
        {
            if (this.isExist())
            {
                string connection = "datasource=localhost;port=3306;username=root;password=root";
                MySqlConnection mysqlconnection = new MySqlConnection(connection);
                string query = "select  * from Data.users where Name= @uname and Password=@upass ;";
                MySqlCommand command = new MySqlCommand(query, mysqlconnection);
                command.Parameters.AddWithValue("@uname", this.name);
                command.Parameters.AddWithValue("@upass", this.password);

                MySqlDataReader reader;
                try
                {
                    mysqlconnection.Open();
                    reader = command.ExecuteReader();
                    int i = 0;
                    while (reader.Read()) i++;

                    if (i == 1)
                    {
                        send("success:Welcome " + this.name);
                        Console.WriteLine("User " + this.name + " logged in");
                        return true;
                    }
                    else
                    {
                        Error("this Wrong username or password");
                        return false;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[!]ERROR: " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    Error("something happend wrong please try again later");
                    return false;
                }
            }
            else
            {
                Error("Username is not Exist Do you want to register?");
                return false;
            }
        }


        #region setters_and_getters
        public void setName(string name)
        {
            this.name = name;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public string getName()
        {
            return this.name;
        }

        public string getPassword()
        {
            return this.password;
        }

        #endregion

        private void Error(string msg)
        {
            stream = client.GetStream();
            buffer = Encoding.UTF8.GetBytes("ERROR:"+msg);
            stream.Write(buffer, 0 , buffer.Length);
            
        }

        private void send(string msg)
        {
            stream = client.GetStream();
            buffer = Encoding.UTF8.GetBytes(msg);
            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine("Sent: " + msg);
        }

        private void Exit()
        {
            client.GetStream().Close();
            client.Close();
            clients.Remove(this);
            Console.WriteLine(this.name + " Disconnected!");
        }

        public string[] getInfo()
        {
            string[] data = new string[7];

            string connection = "datasource=localhost;port=3306;username=root;password=root";
            MySqlConnection mysqlconnection = new MySqlConnection(connection);
            string query = "select  * from Data.PlayersData where Name=@uname;";
            MySqlCommand command = new MySqlCommand(query, mysqlconnection);
            command.Parameters.AddWithValue("@uname", this.name);
           

            MySqlDataReader reader;
            try
            {
                mysqlconnection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < 8; i++)
                        data[i] = reader[i].ToString();
                }

            }
            catch { }
            return data;
        }
    }
}
