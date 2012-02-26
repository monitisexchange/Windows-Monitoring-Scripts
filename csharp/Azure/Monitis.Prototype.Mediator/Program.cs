using System;
using System.Windows.Forms;

namespace Monitis.Prototype.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationContext applicationContext = new ApplicationContext();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HostFrom());
        }
    }
}
