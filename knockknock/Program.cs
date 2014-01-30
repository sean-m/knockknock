using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Data;

namespace knockknock
{
    class Program
    {
        static void Main(string[] args) {
            #if DEBUG
            var preColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("** Debug Build **");
            Console.ForegroundColor = preColor;
            #endif

            string host = "localhost";
            string ports = string.Empty;
            int startPort = 0;
            int endPort = 0;
            bool showPortInfo = false;
            IList<object[]> portInfo;


            DataSet ds = new DataSet();
            
            var fsXml = new StreamReader(System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceStream("knockknock.Ports.xml"));

            ds.ReadXml(fsXml);

            DataTable records = ds.Tables["record"];

            IEnumerable<DataRow> query = from record in records.AsEnumerable()
                                         where record.Field<string>("number") == "80"
                                         select record;           

#region set options
            if (args.Length != 0) {
                try {
                  host = args[0];
                  ports = args[1];

                  if (ports.Contains(':')) {
                      var portRange = ports.Split(new char[] { ':' });
                      var startPortSuccess = System.Int32.TryParse(portRange[0], out startPort);
                      var endPortSuccess = System.Int32.TryParse(portRange[1], out endPort);
                      if (startPortSuccess == false || endPortSuccess == false) { ShowHelp(); }
                  }
                }
                catch {
                    ShowHelp();
                }

            } else {
                ShowHelp(0);
            }

            if (args.Length > 2) {
                switch( args[2].ToLower()) {
                    case "-i":
                        showPortInfo = true;
                        portInfo = Ports();
                        break;
                    case "-info":
                        showPortInfo = true;
                        portInfo = Ports();
                        break;
                    default:
                        break;
                }
            }

#endregion //set options    

            if (endPort != 0) {
                var socket = new TcpClient();
                Console.WriteLine("Checking host: {0}", host);
                for (int i = startPort; i <= endPort; i++) {
                    try {
                        socket.Connect(host, i);
                        Console.WriteLine("Port {0} open", i.ToString());
                    }
                    catch {
                        Console.WriteLine("Port {0} closed", i.ToString());
                    }
                }
            }
            else {
                var singlePort = System.Int32.TryParse(ports, out startPort);
                if (singlePort) {
                    var socket = new TcpClient();
                    Console.WriteLine("Checking host: {0}", host);
                    try {
                        socket.Connect(host, startPort);
                        Console.WriteLine("Port {0} open", startPort.ToString());
                    }
                    catch {
                        Console.WriteLine("Port {0} closed", startPort.ToString());
                    }
                }
            }

            
            #if DEBUG
            Console.Read();
            #endif

        }

        private static IList<object[]> Ports() {
            var ports = new List<object[]>();


            return ports;
        }

        private static void ShowHelp(int exitCode = 0) {
            Console.WriteLine("");
            Console.WriteLine("Usage: knockknock target_name <port|[start_port:end_port]>");
            Console.WriteLine("");
            System.Environment.Exit(exitCode);
        }

       
    }
}
