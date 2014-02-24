/*!
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
		static string appname = "cheats";

		static Config config = new Config();
		static EnvironmentVariables envars = new EnvironmentVariables();

		static bool __debug;
		static bool __pause;
		static string cheatsPath;
		static bool saveToConfig;
		//static bool showHeader;
		//static bool setShowHeader;
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
			//setShowHeader = false;
			setForceDownload = false;
			sheetName = "";

			// Apply settings from config first, then check envars (if allowed).
			__debug = config.attr<bool>("debug");
			cheatsPath = config.contains("cheats-path") ? config.attr<string>("cheats-path") : Path.Combine(envars.global<string>("AppData"), ".cheats");
			//showHeader = config.contains("show-header") ? config.attr<bool>("show-header") : envars.attr<bool>("header", true);
			forceDownload = envars.attr<bool>("force-download", false);
			urls = config.contains("urls") ? config.attr<List<string>>("urls") : envars.attr<List<string>>("urls", new List<string>(new string[] { "https://raw.github.com/kodybrown/cheats/master/files/" }));

			if (!config.exists() && !saveToConfig) {
				// create the config file, the first time..
				config.write();
			}

			for (int i = 0; i < args.Length; i++) {
				if (args[i].Equals("/?") || args[i].Equals("--help", StringComparison.CurrentCultureIgnoreCase)) {
					showUsage();
					if (__pause) {
						ConsolePrompts.PressAnyKey();
					}
					return 0;
				} else if (!handleCommand(args[i])) {
					sheetName = args[i];
				}
			}

			if (saveToConfig) {
				//if (setShowHeader) {
				//	config.attr<bool>("show-header", showHeader);
				//}
				if (setForceDownload) {
					config.attr<bool>("force-download", forceDownload);
				}
				config.write();
			}

			if (sheetName.Length > 0) {
				if (!validateFile(sheetName)) {
					return 1;
				}

				//string filename = "cat.exe";
				//string arguments = "-w --force-plugin:markdown \"" + Path.Combine(cheatsPath, sheetName) + "\"";
				//new Command().run(filename, arguments);

				cat.CatOptions catOptions = new cat.CatOptions();

				catOptions.ignoreBlankLines = false;
				catOptions.ignoreWhitespaceLines = false;
				catOptions.pauseAfterEachPage = false;
				catOptions.pauseAtEnd = false;
				catOptions.showLineNumbers = false;
				catOptions.wrapText = true;

				catOptions.forceSpecificPlugin = "markdown";

				catOptions.files = new List<string>();
				catOptions.files.Add(Path.Combine(cheatsPath, sheetName));

				return new cat.cat().run(catOptions);
			}

			return 0;
		}

		public static bool handleCommand( string a )
		{
			string al;

			while (a.StartsWith("/") || a.StartsWith("-")) {
				a = a.Substring(1);
			}
			al = a.ToLowerInvariant();

			if (al.Equals("debug")) {
				__debug = true;
			} else if (al.Equals("!debug")) {
				__debug = false;
			} else if (al.Equals("p") || al.Equals("pause")) {
				__pause = true;
			} else if (al.Equals("config")) {
				saveToConfig = true;
				//} else if (al.Equals("header") || al.Equals("showheader") || al.Equals("show-header")) {
				//	showHeader = true;
				//	setShowHeader = true;
				//} else if (al.StartsWith("!header") || al.StartsWith("!showheader") || al.StartsWith("!show-header")
				//		|| al.StartsWith("hideheader") || al.StartsWith("hide-header")) {
				//	showHeader = false;
				//	setShowHeader = true;
			} else if (al.Equals("force") || al.Equals("force-download")) {
				forceDownload = true;
				setForceDownload = true;
			} else if (al.Equals("ls") || al.Equals("list") || al.Equals("listlocal") || al.Equals("list-local")) {
				listLocal();
			} else if (al.Equals("ls-server") || al.Equals("listserver") || al.Equals("list-server")) {
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

			string[] files = Directory.GetFiles(cheatsPath, "*.*", SearchOption.AllDirectories);

			if (files.Length > 0) {
				foreach (string f in files) {
					console.writeln(Path.GetFileName(f));
				}
			} else {
				console.writeln(ConsoleColor.DarkCyan, "--none--");
			}
		}

		public static void listServer()
		{
			if (__debug) {
				Console.WriteLine("listServer()");
			}

			string[] files = Directory.GetFiles(cheatsPath, "*.*", SearchOption.AllDirectories);

			if (files.Length > 0) {
				foreach (string f in files) {
					console.writeln(Path.GetFileName(f));
				}
			} else {
				console.writeln(ConsoleColor.DarkCyan, "--none--");
			}
		}

		public static void removeSheet( string name )
		{
			if (__debug) {
				Console.WriteLine("removeSheet('" + name + "')");
			}

			if (sheetName.Length > 0) {
				string tmp = Path.Combine(cheatsPath, sheetName);
				if (File.Exists(tmp)) {
					File.SetAttributes(tmp, FileAttributes.Normal);
					File.Delete(tmp);
					console.writeln(ConsoleColor.Cyan, "deleted `{0}`", sheetName);
				}
			}
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
			int w = Console.WindowWidth,
				indent = 18;


			if (__debug) {
				//Console.WriteLine("");
			}

			console.writeln(ConsoleColor.White, "{0}.exe", appname);
			console.writeln("Created 2009-2013 @wasatchwizard");
			console.writeln();
			console.writeln(ConsoleColor.White, "USAGE:");
			console.writeln(Text.Wrap(string.Format("  {0} [options][sheet-name]", appname), w, 0));
			console.writeln();
			console.writeln(ConsoleColor.White, "OPTIONS:");
			console.writeln(Text.Wrap("  --debug         Outputs additional details while processing.", w, 0, indent));
			console.writeln(Text.Wrap("  --pause         Pause at the end of processing.", w, 0, indent));
			console.writeln();
			//console.writeln(Text.Wrap("  --config        Saves the current specified settings.", w, 0, indent));
			//console.writeln();
			console.writeln(Text.Wrap("  --ls            Lists the local cheat sheets.", w, 0, indent));
			console.writeln(Text.Wrap("  --ls-server     Lists the cheat sheets available online, by source.", w, 0, indent));
			//console.writeln(Text.Wrap("  --header        Outputs a header.", w, 0, indent));
			console.writeln();
			console.writeln(Text.Wrap("  --create        Creates a cheat sheet locally using the specified `sheet-name`.", w, 0, indent));
			console.writeln(Text.Wrap("  --push          Submits (uploads) the specified `sheet-name`. It will be created on the server, if it doesn't already exist.", w, 0, indent));
			console.writeln(Text.Wrap("  --revert        Reverts a local cheat sheet using the specified `sheet-name` (functionally equivalent to --force).", w, 0, indent));
			console.writeln(Text.Wrap("  --remove        Deletes a local cheat sheet using the specified `sheet-name`.", w, 0, indent));
			console.writeln();
			console.writeln(Text.Wrap("  --force         Download the cheat sheet from online, even if it is already local (functionally equivalent to --revert).", w, 0, indent));
			console.writeln(Text.Wrap("  --addsrc:url    Adds the specified `url` to the list of sources.", w, 0, indent));
			console.writeln(Text.Wrap("  --remsrc:url    Removes the specified `url` from the list of sources.", w, 0, indent));

		}
	}
}
