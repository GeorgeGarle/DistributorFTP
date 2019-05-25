using FluentFTP;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributor.Common.Debug;

namespace Distributor.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FtpClient client;

        // Can move this all into a config so it can be changed rather than hard coded
        // The Directory Folder for the FTP Server
        private string _graphicsUpdateLocation = "Graphics/";
        private string _graphicsInstallLocation = "C:/Graphics/";
        private string _graphicsPackage = "DrillSIM2018";

        public MainWindow()
        {
            InitializeComponent();
            Log.SetupLogger(LogType.Client);
        }

        private void GetFiles()
        {
            string[] folderNames;
            string[] fileNames;
            var storage = client.GetListing("/_root");
        }

        // https://stackoverflow.com/questions/1589930/so-what-is-the-right-direction-of-the-paths-slash-or-under-windows
        // FTP Server uses UNIX formatting currently i.e. / NOT \

        private void TestFTP_Click(object sender, RoutedEventArgs e)
        {

            Directory.CreateDirectory("Graphics");
            Directory.CreateDirectory("Output");
            Directory.CreateDirectory("Temp");

            // create an FTP client
            client = new FtpClient("127.0.0.1");

            // if you don't specify login credentials, we use the "anonymous" user account
            client.Credentials = new NetworkCredential("anonymous", "password");
            client.Connect();

            MemoryStream versionStream = new MemoryStream();
            var versionFile = client.Download(versionStream, $"{_graphicsUpdateLocation}version.txt"); //{_graphicsUpdateLocation}
            var version = Encoding.UTF8.GetString(versionStream.ToArray());

            // Only for testing currently
            //GetFiles();

            bool versionsMatch = false;

            if(File.Exists("Graphics/version.txt"))
            {
                var localVersion = File.ReadAllText("Graphics/version.txt");
                versionsMatch = localVersion == version;
            }

            if(!versionsMatch)
            {
                var files = client.GetListing($"{_graphicsUpdateLocation}{version}");
                // Take the first (and hopefully only) .zip

                var file = files.FirstOrDefault(x => x.Name.ToLower().Contains(".zip"));

                // Currently overwrites any existing folders
                FileStream fileStream = new FileStream($"Temp/{file.Name}", FileMode.OpenOrCreate);

                fileStream.SetLength(0);

                var success = client.Download(fileStream, $"{_graphicsUpdateLocation}{version}/{file.Name}");

                fileStream.Close();

                //var success = client.DownloadFile("Temp", $"{version}/{file.Name}", true, FtpVerify.Retry);

                if(success)
                {
                    Directory.Move("Output", $"Output_{DateTime.Now.DayOfYear}_{DateTime.Now.Minute}");
                    Directory.CreateDirectory("Output");

                    using (ZipFile zip = ZipFile.Read($"Temp/{file.Name}"))
                    {
                        zip.ExtractAll("Output");
                    }

                    // everything complete, update the version
                    File.WriteAllText("Graphics/version.txt", version);
                }


            } else
            {

                // Quit!
            }


            //var listing = client.GetListing();

        }
    }
}
