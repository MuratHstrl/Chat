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
namespace Mesajlaşma
{
    public partial class Anasayfa : Form
    {
        Socket sc;
        EndPoint yerel, global;
        bool acıkmı = true;
        bool webacık = false;
        public Anasayfa()
        {
            InitializeComponent();
            sc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sc.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textBox2.Text = Ip();
            textBox5.Text = Ip();
        }
        
        private string Ip()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress pip in host.AddressList)
            {
                if (pip.AddressFamily==AddressFamily.InterNetwork)
                {
                    return pip.ToString();
                }
            }
            return "0";
        }
        private void mesajgeri (IAsyncResult a)
        {
            
            try
            {
                int boyut = sc.EndReceiveFrom(a, ref global);
                if (boyut>0)
                {
                    byte[] sa = new byte[1464];
                    sa = (byte[])a.AsyncState;

                    ASCIIEncoding e = new ASCIIEncoding();
                    string tekrar = e.GetString(sa);
                    Chat.Items.Add(textBox5.Text + " K: " +tekrar);
                    if (acıkmı==false)
                    {
                        notifyIcon1.ShowBalloonTip(3000, "Mesajjın Var !!", tekrar, ToolTipIcon.Info);
                    }
                    
                }
                byte[] buffer = new byte[1500];
                sc.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref global, new AsyncCallback(mesajgeri), buffer);
            }
            catch (Exception ap)
            {

                MessageBox.Show(ap.ToString());
            }
        }
        private void Kapat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Gönder_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding en = new System.Text.ASCIIEncoding();
                byte[] bak = new byte[1500];
                bak = en.GetBytes(textBox1.Text);
                sc.Send(bak);
                Chat.Items.Add(textBox2.Text+" : "+textBox1.Text);
                textBox1.Clear();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void Anasayfa_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            acıkmı = false;
            this.Hide();
        }

        private void açToolStripMenuItem_Click(object sender, EventArgs e)
        {
            acıkmı = true;
            this.Show();
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {

            this.Show();
            acıkmı = true;
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.Show();
            acıkmı = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (webacık==false)
            {
                webBrowser1.Visible = true;
                webacık = true;

            }
            else if (webacık==true)
            {
                webBrowser1.Visible = false;
                webacık = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                yerel = new IPEndPoint(IPAddress.Parse(textBox2.Text), Convert.ToInt32(textBox3.Text));
                sc.Bind(yerel);

                global = new IPEndPoint(IPAddress.Parse(textBox5.Text), Convert.ToInt32(textBox6.Text));
                sc.Connect(global);

                byte[] buffer = new byte[1500];
                sc.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref global, new AsyncCallback(mesajgeri),buffer);

                button1.Text = "Bağlandı";
                button1.Enabled = false;
                button2.Enabled = true;
                Gönder.Enabled = true;
                pictureBox1.Enabled = true;
                textBox1.Focus();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}
