using eShopWinForms.Controllers;
using eShopWinForms.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eShopWinForms
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

            CatalogView catalogView = new CatalogView();
            var grpcAddress = ConfigurationManager.AppSettings["GrpcServiceAddress"] ?? "localhost";
            var grpcPort = int.TryParse(ConfigurationManager.AppSettings["GrpcServicePort"], out var p) ? p : 5001;
            GrpcCatalogService service = new GrpcCatalogService(grpcAddress, grpcPort);
            CatalogController catalogController = new CatalogController(service, catalogView);

            catalogController.LoadView();
            catalogView.ShowDialog();
        }
    }
}
