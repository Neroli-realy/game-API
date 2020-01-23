using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace API_Server
{
    class Player : Creature
    {
        /// <summary>
        /// list of connected players
        /// </summary>
        public static List<Player> Connected = new List<Player>();

        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer;
        public string password;
        public int mana { get; set; }
        public int Gold { get; set; }
        public int Level { get; set; }

        public List<Monster> monsters { get; set; }

        public List<Player> Freinds { get; set; }
        public Player(TcpClient client)
        {
            //create player and start communication with him
            this.client = client;
            Communicate();

        }

        private void Communicate()
        {
            //get requests from client
            if (client.Connected)
            {
                stream = this.client.GetStream();
                buffer = new byte[1024];
                //async to avoid block 
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(callBack), stream);
            }
        }

        private void callBack(IAsyncResult res)
        {
            //get request drom client
            stream = (NetworkStream)res.AsyncState;
            int size = stream.EndRead(res);
            string data = Encoding.UTF8.GetString(buffer, 0, size);

            if (data.StartsWith("LOGIN:"))
            {
                this.name = data.Split(':')[1];
                this.password = data.Split(':')[2];
                Commands.doLogin(this);
            }
            else if (data.StartsWith("REG:"))
            {
                this.name = data.Split(':')[1];
                this.password = data.Split(':')[2];
                Commands.doREG(this);
            }
            else if (data.StartsWith("GETUSERS")) Commands.getUsers(this);
            else if (data.StartsWith("EXIT")) Exit();
            else if (data.StartsWith("ADD:"))
            {
                string n = data.Split(':')[1];
                Commands.sendAdd(this, n);
            }
            else if (data.StartsWith("LOGOUT")) Logout();
            else if (data.StartsWith("ACC:")) Commands.acceptAdd(this, data.Split(':')[1]);
            else if (data.StartsWith("FR")) Commands.sendFR(this);
            else if (data.StartsWith("MONS"))  Commands.getMonsters(this);
               
            //relisten to other requests
            Communicate();
        }

        public void send(string msg)
        {
            stream = client.GetStream();
            buffer = Encoding.UTF8.GetBytes(msg);
            stream.Write(buffer, 0, buffer.Length);
            Alerts.Debug(msg);
        }

        private void Logout()
        {
            Connected.Remove(this);
            send(this.name +" logged out!");
        }
        private void Exit()
        {
            client.GetStream().Close();
            client.Close();
            Connected.Remove(this);
            Alerts.INFO(this.name + " Disconnected!");
        }
    }
}
