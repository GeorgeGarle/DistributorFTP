using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributor.Common.Debug;
using Distributor.Server.FTP.lib;

namespace Distributor.Server
{
    class DistributorServer
    {

        public static DistributorServer Server;
        public static FTPServer ftpServer;

        private static bool _running = true;

        public DistributorServer()
        {

            Server = this;
            Log.SetupLogger();

            Init();

        }

        public void Init()
        {

            var args = Environment.GetCommandLineArgs();

            bool verbose = false;
            bool anyPeer = false;
            int port = -1;
            int buffer = -1;

            // process args
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-h" || args[i] == "--help")
                {
                    Console.Out.WriteLine("Usage: <program> [-v|--verbose] [-p|--port <port>] [-b|--buffer <kbsize>] [-a|--any-peer]");

                }
                else if (args[i] == "-v" || args[i] == "--verbose")
                {
                    verbose = true;
                }
                else if (args[i] == "-a" || args[i] == "--any-peer")
                {
                    anyPeer = true;
                }
                else if (args[i] == "-p" || args[i] == "--port")
                {
                    if (i == args.Length - 1)
                    {
                        Console.Error.WriteLine("Too few arguments for {0}", args[i]);
                    }

                    port = ParseNumber(args[i], args[i + 1]);
                    if (port == -1)
                    {
                        Console.Error.WriteLine("Invalid value for '{0}': {1}", args[i], args[i + 1]);
                    }

                    ++i;
                }
                else if (args[i] == "-b" || args[i] == "--buffer")
                {
                    if (i == args.Length - 1)
                    {
                        Console.Error.WriteLine("Too few arguments for {0}", args[i]);
                    }

                    buffer = ParseNumber(args[i], args[i + 1]);
                    if (buffer == -1)
                    {
                        Console.Error.WriteLine("Invalid value for '{0}': {1}", args[i], args[i + 1]);
                    }

                    ++i;
                }
                else
                {
                    Console.Error.WriteLine("Unknown argument '{0}'", args[i]);
                }
            }

            ftpServer = new FTPServer();

            ftpServer.LogHandler = new DefaultLogHandler(verbose);
            ftpServer.AuthHandler = new DefaultAuthHandler(anyPeer);

            if (port != -1)
                ftpServer.LocalPort = port;

            if (buffer != -1)
                ftpServer.BufferSize = buffer * 1024; // in KB

            Console.Out.WriteLine("Starting server on {0}", ftpServer.LocalEndPoint);

            try
            {
                ftpServer.Run();

                while (_running)
                {

                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                ftpServer.Stop();
            }
            

        }

        ~DistributorServer()
        {

        }

        private static int ParseNumber(string option, string text)
        {
            try
            {
                // CF does not have int.TryParse
                int num = int.Parse(text);
                if (num > 0)
                    return num;
            }
            catch (Exception)
            {
            }

            return -1;
        }


    }
}
