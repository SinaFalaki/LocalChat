using System;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPServer
{
    public partial class FrmServer : Form
    {
        Socket SocServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket SocClient = null;
        public SoundPlayer rington = new SoundPlayer();

        public FrmServer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public void GetMsg()
        {
            try
            {
                while (true)
                {
                    byte[] barray = new byte[1024];
                    int RecB = SocClient.Receive(barray);
                    if (RecB > 0)
                    {
                        txtInfo.Text += $"کلاینت ⌚ {DateTime.Now.ToString("HH:mm")}: {Encoding.Unicode.GetString(barray, 0, RecB)}";
                        txtInfo.Text += $"{Environment.NewLine}";

                        //scroll down
                        txtInfo.SelectionStart = txtInfo.Text.Length;
                        txtInfo.ScrollToCaret();
                        txtInfo.Focus();

                        rington.Play();

                    }
                }
            }
            catch
            {
                MessageBox.Show("مشکل!");
            }
        }
        public void StartServer()
        {
            string[] parts = txtIP.Text.Split(':');
            IPEndPoint ipendpointserver = new IPEndPoint(IPAddress.Any, int.Parse(parts[1]));
            SocServer.Bind(ipendpointserver);
            SocServer.Listen(1);
            txtInfo.Text += $"{Environment.NewLine}";
            txtInfo.Text += $"Starting...{Environment.NewLine} ";
            btnStart.Enabled = false;
            btnSend.Enabled = true;
            SocClient = SocServer.Accept();
            Thread trGetMsg = new Thread(new ThreadStart(GetMsg));
            trGetMsg.Start();
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Thread TrStart = new Thread(new ThreadStart(StartServer));
            TrStart.Start();

            //read from data base
            txtInfo.Text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ServerStorage.txt");

            //scroll down
            txtInfo.SelectionStart = txtInfo.Text.Length;
            txtInfo.ScrollToCaret();
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


        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] barray = new byte[1024];
            if (!string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                barray = Encoding.Unicode.GetBytes(txtMessage.Text);
                SocClient.Send(barray);
                txtInfo.Text += $"من ⌚ {DateTime.Now.ToString("HH:mm")}: ";
                txtInfo.Text += $"{txtMessage.Text}{Environment.NewLine}";
                txtMessage.Text = string.Empty;

                //scroll down
                txtInfo.SelectionStart = txtInfo.Text.Length;
                txtInfo.ScrollToCaret();
                // txtInfo.Focus();
            }


        }


        #region hotkey
        private void FrmServer_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                //START disable default work of Enter Key
                e.SuppressKeyPress = true;
                //END disable default work of Enter Key
                btnSend.PerformClick();
                txtMessage.Text = string.Empty;
            }

        }

        #endregion

        private void FrmServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtInfo.Text += $"{Environment.NewLine}";
            txtInfo.Text += $"                        --=*شما از چت خارج شدید*=--{Environment.NewLine}";

            byte[] barray = new byte[1024];
            barray = Encoding.Unicode.GetBytes($"سیـــنا از چت خارج شد 😢{Environment.NewLine}");
            SocClient.Send(barray);


            //scroll down
            txtInfo.SelectionStart = txtInfo.Text.Length;
            txtInfo.ScrollToCaret();
            //save data base
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "ServerStorage.txt", txtInfo.Text);

            try
            {
                if (SocClient != null)
                {
                    SocClient.Shutdown(SocketShutdown.Both);

                }
                if (SocServer != null)
                {
                    SocServer.Shutdown(SocketShutdown.Both);
                }
                Environment.Exit(Environment.ExitCode);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(Environment.ExitCode);
                Application.Exit();

            }
        }
    }
}
