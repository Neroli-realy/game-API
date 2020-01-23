using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using API_Server;

namespace Simple_Client
{
    public partial class Form1 : Form
    {
        private NetworkStream stream;
        private byte[] buffer;
        public static TcpClient cli;
        public ControlPanel panel;
        public static string[] players;
        public static string[] fr;
        public static List<Monster> Monsters = new List<Monster>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text == "Login")
                {
                    NetworkStream stream = cli.GetStream();
                    string data = "LOGIN:" + textBox1.Text + ":" + textBox2.Text;
                    var buffer = Encoding.UTF8.GetBytes(data);
                    stream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    NetworkStream stream = cli.GetStream();
                    string data = "LOGOUT";
                    var buffer = Encoding.UTF8.GetBytes(data);
                    stream.Write(buffer, 0, buffer.Length);
                    panel.Close();
                }
              //  panel = new ControlPanel();
                
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Getdata()
        {
            if (cli.Connected)
            {
               stream = cli.GetStream();
               buffer = new byte[1000];
               stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReaCB), stream);
            }
        }
        private void ReaCB(IAsyncResult res)
        {
           /* try
            {*/
                stream = (NetworkStream)res.AsyncState;
                var Ab = stream.EndRead(res);
                string data = Encoding.UTF8.GetString(buffer, 0, Ab);
                if (data.StartsWith("INFO:"))
                {
                    lblHP.Invoke(new Action(() => lblHP.Text += " " + data.Split(':')[2]));
                    lblMana.Invoke(new Action(() => lblMana.Text += " " + data.Split(':')[4]));
                    lblLevel.Invoke(new Action(() => lblLevel.Text += " " + data.Split(':')[7]));
                    lblGold.Invoke(new Action(() => lblGold.Text += " " + data.Split(':')[6]));
                }
                else if (data.StartsWith("ERROR:"))
                {
                    richTextBox1.Invoke(new Action(() => richTextBox1.Text += "ERROR: " + data.Split(':')[1] + "\n"));
                }
                else if (data.StartsWith("Success:"))
                {
                    richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Done: " + data.Split(':')[1] + "\n"));
                    if (data.Contains("Welcome"))
                    {
                        button1.Invoke(new Action(() => button1.Text = "Logout"));
                        textBox1.Invoke(new Action(() => textBox1.Enabled = false));
                        textBox2.Invoke(new Action(() => textBox2.Enabled = false));
                        button2.Invoke(new Action(() => button2.Enabled = false));
                        menuStrip1.Invoke(new Action(() => chatToolStripMenuItem.Enabled = true));
                    }
                }
                else if (data.StartsWith("PLAYERS"))
                {
                    players = data.Split(':');
                }
                else if (data.StartsWith(textBox1.Text))
                {

                    button1.Invoke(new Action(() => button1.Text = "Login"));
                    textBox1.Invoke(new Action(() => textBox1.Enabled = true));
                    textBox2.Invoke(new Action(() => textBox2.Enabled = true));
                    button2.Invoke(new Action(() => button2.Enabled = true));
                    menuStrip1.Invoke(new Action(() => chatToolStripMenuItem.Enabled = false));
                    lblHP.Invoke(new Action(() => lblHP.Text = "HP:"));
                    lblGold.Invoke(new Action(() => lblGold.Text = "Gold:"));
                    lblLevel.Invoke(new Action(() => lblLevel.Text = "Level:"));
                    lblMana.Invoke(new Action(() => lblHP.Text = "Mana:"));

                }
                else if (data.StartsWith("FR")) fr = data.Split(':');
                else if (data.StartsWith("MONST:"))
                {
                    Monster m = new Monster(data.Split(':')[1], data.Split(':')[2], data.Split(':')[3], data.Split(':')[4], data.Split(':')[5]);
                    Monsters.Add(m);
                }
                else if (data.StartsWith("END")) panel.getmonst();
                else
                    richTextBox1.Invoke(new Action(() => richTextBox1.Text += "msg: " + data + "\n"));
                
                Getdata();
            /*}
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }*/
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(cli.Connected)
            {
                NetworkStream stream = cli.GetStream();
                string data = "REG:" + textBox1.Text + ":" + textBox2.Text;
                var buffer = Encoding.UTF8.GetBytes(data);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cli.Connected)
            {
                NetworkStream stream = cli.GetStream();
                string data = "EXIT";
                var buffer = Encoding.UTF8.GetBytes(data);
                stream.Write(buffer, 0, buffer.Length);
                cli.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cli = new TcpClient();
            try
            {
                while(cli.Connected == false)
                    cli.Connect("localhost", 8686);
                Getdata();
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        

        private void chatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel = new ControlPanel();
            panel.Show();
        }

        private void chatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Chat comuunity = new Chat();
            comuunity.Show();
        }

       
       
    }
}
