using System;
using System.Windows.Forms;
using MiniMarket.Services;

namespace MiniMarket
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                
                Database.Initialize();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(
                    "Error al inicializar la base de datos:\n\n" + ex.Message,
                    "Error BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            
            Application.Run(new LoginForm());

            
        }
    }
}
