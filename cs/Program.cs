﻿/*!
	Copyright (C) 2013 Kody Brown (kody@bricksoft.com).
	
	MIT License:
	
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to
	deal in the Software without restriction, including without limitation the
	rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
	sell copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	
	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
	DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Bricksoft.PowerCode;

namespace cheats
{
	public class Program
	{
		static Config config = new Config();
		static EnvironmentVariables envars = new EnvironmentVariables();

		static string appname = "cheats";

		static bool __debug;
		static bool __pause;
		static string cheatsPath;
		static bool saveToConfig;
		static bool showHeader;
		static bool setShowHeader;
		static bool forceDownload;
		static bool setForceDownload;
		static List<string> urls;
		static string sheetName;

		public static int Main( string[] args )
		{
			config.read(true);
			envars.prefix = appname;

			if (args.Length == 0) {
				showUsage();
				return 0;
			}

			// Apply default settings, that cannot be saved in config, nor set in envars.
			__pause = false;
			saveToConfig = false;
			setShowHeader = false;
			setForceDownload = false;
			sheetName = "";

			// Apply settings from config first, then check envars.
			__debug = config.attr<bool>("debug");
			cheatsPath = config.attr<string>("cheatsPath", Path.Combine(envars.attr<string>("AppData"), ".cheats"));
			showHeader = config.contains("showHeader") ? config.attr<bool>("showHeader") : envars.attr<bool>("header", true);
			forceDownload = config.contains("forceDownload") ? config.attr<bool>("forceDownload") : envars.attr<bool>("force", true);
			urls = config.contains("urls") ? config.attr<List<string>>("urls") : envars.attr<List<string>>("urls", new List<string>(new string[] { "https://raw.github.com/kodybrown/cheats/master/files/" }));

			if (!config.exists() && !saveToConfig) {
				// create the config file, the first time..
				config.write();
			}

			for (int i = 0; i < args.Length; i++) {
				if (!handleCommand(args[i])) {
					sheetName = args[i];
				}
			}

			if (saveToConfig) {
				if (setShowHeader) {
					config.attr<bool>("showHeader", showHeader);
				}
				if (setForceDownload) {
					config.attr<bool>("forceDownload", forceDownload);
				}
				config.write();
			}

			if (!validateFile(sheetName)) {
				return 1;
			}

			string filename = "cat.exe";
			string arguments = "-w --force-plugin:markdown \"" + Path.Combine(cheatsPath, sheetName) + "\"";

			new Command().run(filename, arguments);

			return 0;
		}

		public static bool handleCommand( string a )
		{
			string al;

			while (a.StartsWith("/") || a.StartsWith("-")) {
				a = a.Substring(1);
			}
			al = a.ToLowerInvariant();

			if (al.Equals("?") || al.Equals("help")) {
				showUsage();
				if (__pause) {
					ConsolePrompts.PressAnyKey();
				}
			} else if (al.Equals("debug")) {
				__debug = true;
			} else if (al.Equals("!debug")) {
				__debug = false;
			} else if (al.Equals("p") || al.Equals("pause")) {
				__pause = true;
			} else if (al.Equals("config")) {
				saveToConfig = true;
			} else if (al.Equals("header") || al.Equals("showheader") || al.Equals("show-header")) {
				showHeader = true;
				setShowHeader = true;
			} else if (al.StartsWith("!header") || al.StartsWith("!showheader") || al.StartsWith("!show-header")
					|| al.StartsWith("hideheader") || al.StartsWith("hide-header")) {
				showHeader = false;
				setShowHeader = true;
			} else if (al.Equals("force") || al.Equals("force-download")) {
				forceDownload = true;
				setForceDownload = true;
			} else if (al.Equals("list") || al.Equals("listlocal") || al.Equals("list-local")) {
				listLocal();
			} else if (al.Equals("listserver") || al.Equals("list-server")) {
				listServer();
			} else if (al.StartsWith("remove:") || al.StartsWith("delete:")) {
				removeSheet(a.Substring(7));
			} else if (al.StartsWith("addsource:")) {
				addSource(a.Substring(10));
			} else {
				return false;
			}

			return true;
		}

		private static void debug( string name, params object[] p )
		{
			if (__debug) {
				StringBuilder s = new StringBuilder();
				s.AppendFormat("DEBUG: {0,-14}", name);
				foreach (object o in p) {
					s.Append(" " + o.ToString());
				}
				Console.WriteLine(s.ToString());
			}
		}

		private static bool validateFile( string sheetName )
		{
			return true;
		}

		private static int downloadFile( string urlToDownload, string fileToSave, bool verbose )
		{
			const int PADDING = 13;
			const int bufferSize = 65536; // 32768, 65536, 131072;

			if (urlToDownload == null || urlToDownload.Length == 0) {
				return 1;
			}
			if (fileToSave == null || fileToSave.Length == 0) {
				fileToSave = Path.GetFileName(urlToDownload);
			}

			WebClient Client = new WebClient();

			if (verbose) {
				Console.WriteLine("Downloading: {0}", urlToDownload);
				Console.WriteLine("             using {0}KB blocks", Convert.ToInt32(bufferSize / 1024));
				Console.Write("   Progress: 0 bytes                           ");
			}

			try {
				Stream inStream;
				BinaryReader sr;
				byte[] fileContents;
				string tempFile;

				inStream = Client.OpenRead(urlToDownload);
				sr = new BinaryReader(inStream);
				fileContents = new byte[bufferSize];
				tempFile = fileToSave + "!";

				if (File.Exists(tempFile)) {
					File.SetAttributes(tempFile, FileAttributes.Normal);
					File.Delete(tempFile);
				}

				FileStream outStream = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.Write);
				BinaryWriter writer = new BinaryWriter(outStream);
				int nBytesRead = 0;
				int nTotalBytesRead = 0;
				int getCount = 0;

				do {
					nBytesRead = sr.Read(fileContents, 0, bufferSize);
					writer.Write(fileContents, 0, nBytesRead);
					nTotalBytesRead += nBytesRead;

					if (verbose) {
						if (getCount++ % 4 == 0) {
							Console.SetCursorPosition(PADDING, Console.CursorTop);
							Console.Write(string.Format("{0:0,0} bytes                              ", nTotalBytesRead));
						}
					}
				} while (nBytesRead > 0);

				if (verbose) {
					// just in case..
					Console.SetCursorPosition(PADDING, Console.CursorTop);
					Console.Write(string.Format("{0:0,0} bytes                                    \r\n", nTotalBytesRead));
				}

				sr.Close();
				sr = null;
				inStream.Close();
				inStream = null;
				writer.Close();
				writer = null;
				outStream.Close();
				outStream = null;

				if (File.Exists(fileToSave)) {
					File.SetAttributes(fileToSave, FileAttributes.Normal);
					File.Delete(fileToSave);
				}
				File.Move(tempFile, fileToSave);

				return 0;
			} catch (Exception ex) {
				Console.Write("...\r\nError Occurred:\r\n\r\n" + ex.Message + "\r\n");
			}

			return 5;
		}

		public static void listLocal()
		{
			if (__debug) {
				Console.WriteLine("listLocal()");
			}
		}

		public static void listServer()
		{
			if (__debug) {
				Console.WriteLine("listServer()");
			}
		}

		public static void removeSheet( string name )
		{
			if (__debug) {
				Console.WriteLine("removeSheet('" + name + "')");
			}

			//sheetPath
		}

		public static void addSource( string url )
		{
			if (__debug) {
				Console.WriteLine("addSource('" + url + "')");
			}

			if (urls.Contains(url)) {
				return;
			}

			urls.Add(url);
			config.attr<List<string>>("urls", urls);

			config.write();
		}

		public static void showUsage()
		{
			int w = Console.WindowWidth - 1;

			if (__debug) {
				Console.WriteLine("cheats.exe");
			}

			Console.WriteLine(Text.Wrap(string.Format("{0}.exe", appname), w, 0));

		}
	}
}
