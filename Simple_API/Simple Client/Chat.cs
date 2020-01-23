using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Simple_Client
{
    public partial class Chat : Form
    {
        public Chat()
        {
            InitializeComponent();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            send("FR");
        }

        void send(string msg)
        {
            NetworkStream stream = Form1.cli.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            stream.Write(buffer, 0, buffer.Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            send("ACC:" + listBox2.SelectedItem.ToString());

        }

        private void button4_Click(object sender, EventArgs e)
        {
            send("DEC:" + listBox2.SelectedItem.ToString());
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            foreach (string s in Form1.fr)
            {
                if (!s.Equals("FR"))
                    listBox2.Items.Add(s);
            }
        }
    }
}
