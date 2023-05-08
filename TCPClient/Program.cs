using System;
using System.Media;
using System.Windows.Forms;

namespace TCPClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmClient());
            SoundPlayer rington = new SoundPlayer();
            rington.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\ring.wav";
        }

    }
}
