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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cheats
{
	public class Command
	{
		public Command()
		{

		}

		public int run( string file, string arguments = "", int timeoutSeconds = 30 )
		{
			Process proc;
			StringBuilder s = new StringBuilder();
			Thread stdOutThread = null;
			Thread stdInThread = null;
			Thread stdErrThread = null;

			proc = new Process();

			proc.StartInfo.FileName = file;
			proc.StartInfo.Arguments = arguments;

			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.ErrorDialog = false;
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			proc.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.RedirectStandardInput = true;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.EnableRaisingEvents = true;

			try {
				proc.Start();

				// works, but minimally, and no support for input, nor error..
				//using (StreamReader o = proc.StandardOutput) {
				//	s.Append(o.ReadToEnd());
				//}
				//Console.WriteLine(s.ToString());

				stdOutThread = new Thread(new ParameterizedThreadStart(HandleStdOut));
				stdOutThread.Start(proc);

				stdErrThread = new Thread(new ParameterizedThreadStart(HandleStdErr));
				stdErrThread.Start(proc);

				stdInThread = new Thread(new ParameterizedThreadStart(HandleStdIn));
				stdInThread.Start(proc);
				
				//while (!proc.HasExited) {
				//	Thread.Sleep(250);
				//}

				proc.WaitForExit(timeoutSeconds * 1000);
				return proc.ExitCode;

			} catch (Exception) {
				// TODO
				return 100;
			} finally {
				if (stdOutThread != null) {
					if (stdOutThread.IsAlive) {
						stdOutThread.Abort();
						stdOutThread.Join(1000);
					}
				}
				if (stdErrThread != null) {
					if (stdErrThread.IsAlive) {
						stdErrThread.Abort();
						stdErrThread.Join(1000);
					}
				}
				if (stdInThread != null) {
					if (stdInThread.IsAlive) {
						stdInThread.Abort();
						stdInThread.Join(1000);
					}
				}
			}

		}

		private void HandleStdOut( object procObject )
		{
			Process proc;
			string stdout;

			proc = (Process)procObject;
			stdout = null;

			while (!proc.HasExited) {
				if (null != (stdout = proc.StandardOutput.ReadLine())) {
					Console.WriteLine(stdout);
				}
			}
		}

		private void HandleStdErr( object procObject )
		{
			Process proc;
			string stderr;

			proc = (Process)procObject;
			stderr = null;

			while (!proc.HasExited) {
				if (null != (stderr = proc.StandardError.ReadLine())) {
					//Console.WriteLine(ConsoleColor.Red, stderr);
					Console.WriteLine(stderr);
				}
			}
		}

		private void HandleStdIn( object procObject )
		{
			Process proc;
			string stdin;

			proc = (Process)procObject;
			stdin = null;

			while (!proc.HasExited) {
				if (null != (stdin = proc.StandardError.ReadLine())) {
					Console.WriteLine(stdin);
				}
			}
		}
	}
}
