using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Diagnostics;
using Gnu.Getopt;


namespace knockknock
{
	class Program
	{
		static void Main (string[] args) {
#if DEBUG
			var preColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine ("** Debug Build **");
			Console.ForegroundColor = preColor;
#endif
			Getopt g = new Getopt ("knockknock", args, "");
			//LongOpt l = new LongOpt ("knockknock", 
			string host = String.Empty;
			List<int> tcpPorts = new List<int> ();
			List<int> udpPorts = new List<int> ();
			Dictionary<int,string> portInfo;

			#region parseArgs
			//TODO (sean) Use getopt for arguments
			#if True
			char c;
			while ((c = (char)g.getopt ()) != -1) {

				switch (c) {
					case 'a':
						break;

				}

			}
			#endif

			host = args [0];

			for (int i = 1; i < args.Length; i++) {
				if (args [i].ToLower () == "-tcp") {
					i++;
					string[] tmp = new string[100];
					tmp = args [i].Split (new char[] { ',' });
					for (int j = 0; j < tmp.Length; j++) {
						if (!String.IsNullOrEmpty (tmp [j]))
							tcpPorts.Add (Convert.ToInt32 (tmp [j]));
					}
				}

				if (args [i].ToLower () == "-udp") {
					i++;
					string[] tmp = new string[100];
					tmp = args [i].Split (new char[] { ',' });
					for (int j = 0; j < tmp.Length; j++) {
						if (!String.IsNullOrEmpty (tmp [j]))
							udpPorts.Add (Convert.ToInt32 (tmp [j]));
					}
				}

				if (args [i].ToLower () == "-i") {

				}
			}			


			#endregion //parseArgs    

			var socket = new TcpClient ();
			var stopwatch = new Stopwatch ();
			string heading = String.Format ("{0}                    {1}   {2}  {3} {4}", "Date", "Status", "Port", "Time", "Type");


			if (tcpPorts.Count > 0) {
				Console.WriteLine (heading);
				PokePorts (socket, stopwatch, host, tcpPorts, "TCP");
			}


			if (udpPorts.Count > 0) {
				foreach (var i in udpPorts) {

					try {

						stopwatch.Start ();
						socket.Connect (host, i);
						socket.Close ();

						LogOutput (String.Format ("UDP Open   {0,5} {1}", i, stopwatch.ElapsedMilliseconds), LogType.good);
						stopwatch.Reset ();
					} catch {
						LogOutput (String.Format ("UDP Closed {0,5} {1}", i, stopwatch.ElapsedMilliseconds), LogType.bad);
						stopwatch.Reset ();
					}
				}

			}


		}


		private static void PokePorts (TcpClient socket, Stopwatch stopwatch, string host, List<int> ports, string conntype) {
			foreach (var i in ports) {

				try {
					stopwatch.Start ();
					socket.Connect (host, i);
					socket.Close ();
					LogOutput (String.Format ("Open   {0,5} {1,6} {2}", i, stopwatch.ElapsedMilliseconds, conntype), LogType.good);
					stopwatch.Reset ();
				} catch {
					LogOutput (String.Format ("Closed {0,5} {1,6} {2}", i, stopwatch.ElapsedMilliseconds, conntype), LogType.bad);
					stopwatch.Reset ();
				}
			}
		}


		private static void LogOutput (string msg, LogType type) {
			Console.Write ("[{0}] ", DateTime.UtcNow);

			switch (type) {
				case LogType.good:
					{
						Console.ForegroundColor = ConsoleColor.Green;
						break;
					}
				case LogType.bad:
					{
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					}
				case LogType.warn:
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					}
				default:
					Console.ResetColor ();
					break;
			}

			Console.WriteLine (msg);
			Console.ResetColor ();
		}

		private static void ShowHelp (int exitCode = 0) {
			Console.WriteLine ("");
			Console.WriteLine ("Usage: knockknock target_name [-tcp 80,443,445...] [-udp 53,161]");
			Console.WriteLine ("");
			System.Environment.Exit (exitCode);
		}


		enum LogType {
			good,
			bad,
			warn,
			info,
		};

	}
}
