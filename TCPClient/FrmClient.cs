using System;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPClient
{
    public partial class FrmClient : Form
    {
        Socket SClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public FrmClient()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        //SimpleTcpClient client;
        public SoundPlayer rington = new SoundPlayer();

        public void getMsg()
        {

            try
            {
                while (true)
                {
                    byte[] b = new byte[1024];
                    int R = SClient.Receive(b);
                    if (R > 0)
                    {
                        txtInfo.Text += $"سـیـــنا ⌚ {DateTime.Now.ToString("HH:mm")}: {Encoding.Unicode.GetString(b, 0, R)}";
                        txtInfo.Text += $"{Environment.NewLine}";

                        //scroll down
                        txtInfo.SelectionStart = txtInfo.Text.Length;
                        txtInfo.ScrollToCaret();
                        //txtInfo.Focus();

                        rington.Play();
                    }
                }
            }
            catch
            {
                MessageBox.Show("مشکل!");
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            byte[] barray = new byte[1024];
            if (!string.IsNullOrEmpty(txtMessage.Text))
            {
                barray = Encoding.Unicode.GetBytes(txtMessage.Text);
                SClient.Send(barray);

                txtInfo.Text += $"خودم ⌚ {DateTime.Now.ToString("HH:mm")}: ";
                txtInfo.Text += $"{txtMessage.Text}{Environment.NewLine}";
                //scroll down
                txtInfo.SelectionStart = txtInfo.Text.Length;
                txtInfo.ScrollToCaret();
                //txtInfo.Focus();

                txtMessage.Text = string.Empty;
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string[] parts = txtIP.Text.Split(':');
            IPEndPoint IpServe = new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
            try
            {
                SClient.Connect(IpServe);
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
                //read from data base
                txtInfo.Text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ClientStorage.txt");
                //scroll down
                txtInfo.SelectionStart = txtInfo.Text.Length;
                txtInfo.ScrollToCaret();

                txtInfo.Text += $"{Environment.NewLine}";
                txtInfo.Text += $"                             شما به چت پیوستید 😄.{Environment.NewLine}";
                rington.Play();

                byte[] barray = new byte[1024];
                barray = Encoding.Unicode.GetBytes($"کلاینت به چت وارد شد.{Environment.NewLine}");
                SClient.Send(barray);

                Thread tr = new Thread(new ThreadStart(getMsg));
                tr.Start();
            }
            catch
            {
                MessageBox.Show("Server  not Start");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            btnSend.Enabled = false;
            rington.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "ring.wav";
            //hotkey
            this.KeyPreview = true;
            //end of hotkey
            txtMessage.Focus();


        }




        //start hotkey
        private void FrmClient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {   //START disable default work of Enter Key
                e.SuppressKeyPress = true;
                // END disable default work of Enter Key
                btnSend.PerformClick();
                txtMessage.Text = string.Empty;
            }

        }
        //end of hotkey

        private void FrmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtInfo.Text += $"{Environment.NewLine}";
            txtInfo.Text += $"                        --=*شما از چت خارج شدید*=--{Environment.NewLine}";

            byte[] barray = new byte[1024];
            barray = Encoding.Unicode.GetBytes($"کلاینت از چت خارج شد{Environment.NewLine}");
            SClient.Send(barray);

            //scroll down
            txtInfo.SelectionStart = txtInfo.Text.Length;
            txtInfo.ScrollToCaret();

            //save data base
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "ClientStorage.txt", txtInfo.Text);

            if (SClient != null)
            {
                SClient.Shutdown(SocketShutdown.Both);
                Environment.Exit(Environment.ExitCode);
                Application.Exit();
            }
        }
    }
}
