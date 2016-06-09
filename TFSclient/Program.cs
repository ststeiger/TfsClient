using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TFSclient
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (false)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }

            // TfsApplication.TfsMain();
            //TfsApplication.TfsItemList();
            //TfsApplication.CrappyKeepTrack();


            string sourcePath = "$/COR-InventoryManagement/";
            string targetPath = @"D:\username\Desktop\noob";
            // TfsApplication.DownloadFiles(sourcePath, targetPath);
            TfsApplication.ListChangesInProject();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            Console.ReadKey();
        }
    }
}
