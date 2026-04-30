using System;
using System.Windows.Forms;

namespace SubscriptionManager
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DatabaseHelper.InitializeDatabase();

            DatabaseHelper.EnsureAdminExists();

            Application.Run(new Form1());
        }
    }
}