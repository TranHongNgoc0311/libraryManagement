using LibraMaster_ver2._0.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraMaster_ver2._0
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
            ProjectLibraryManagementDataContext db = new ProjectLibraryManagementDataContext();
            if (Properties.Settings.Default.Email != string.Empty)
            {
                UserLoginData.Userdata = db.Accounts.SingleOrDefault(x => x.email == Properties.Settings.Default.Email);

                Application.Run(new Waiting());
            }
            else
            {
                Application.Run(new LoginForm());
            }
        }
    }
}
