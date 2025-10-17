using System;
using System.Windows.Forms;
using WinFormApp.Models;
using WinFormApp.Utils;

namespace WinFormApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());

            using (var context = new CoffeeShopContext())
            {
                context.Database.EnsureCreated();
                DataSeeder.Seed(context);
            }
        }
    }
}
