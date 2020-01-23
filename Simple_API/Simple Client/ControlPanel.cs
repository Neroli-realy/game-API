using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using API_Server;

namespace Simple_Client
{
    public partial class ControlPanel : Form
    {
        private  NetworkStream stream;
        private  byte[] buffer = new byte[1024];
        public ControlPanel()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {
            getusers();
            send("MONS");
            

        }

        private void frindsList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            send("ADD:" + frindsList.SelectedItem.ToString());
        }
        private void send(string request)
        {
            stream = Form1.cli.GetStream();
            string data = request;
            buffer = Encoding.UTF8.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
        }
        private  void getusers()
        {
            try
            {
                send("GETUSERS");
                frindsList.Invoke(new Action(() => frindsList.Items.Clear()));
                foreach (string s in Form1.players)
                {
                    if (!String.IsNullOrEmpty(s) && s != "PLAYERS" )
                        frindsList.Invoke(new Action(() => frindsList.Items.Add(s)));
                }
            }
            catch { }
        }

        public void getmonst()
        {

            foreach (Monster m in Form1.Monsters)
            {
               comboBox2.Invoke(new Action(() => comboBox2.Items.Add(m.name)));
               
            }
        }
        private void frindsList_Click(object sender, EventArgs e)
        {
            getusers();
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Monster m in Form1.Monsters)
            {
                if (m.name.Equals(comboBox2.SelectedItem.ToString()))
                {
                    mnsDEF.Text = "Defense: " + m.def;
                    mstHP.Text = "HP: " + m.health;
                    mnsMR.Text = "MagicResistance: " + m.mr;
                }
            }
        }

       

       
    }
}
