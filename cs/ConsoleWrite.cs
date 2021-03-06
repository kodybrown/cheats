/*!
	Copyright (C) 2006-2013 Kody Brown (kody@bricksoft.com).
	
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
using System.Globalization;
using System.Runtime.InteropServices;

public static class console
{
	#region ----- write() -----

	public static void write( object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		write(Convert.ToString(value, CultureInfo.CurrentCulture));
	}

	public static void write( CultureInfo cultureInfo, object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		write(Convert.ToString(value, cultureInfo));
	}

	public static void write( string value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		Console.Write(value);
	}

	public static void write( string format, params object[] values )
	{
		write(CultureInfo.CurrentCulture, format, values);
	}

	public static void write( CultureInfo cultureInfo, string format, params object[] values )
	{
		if (format == null) {
			throw new ArgumentNullException("format");
		} else if (format.Trim().Length == 0) {
			throw new ArgumentException("", "format");
		}
		if (values == null) {
			throw new ArgumentNullException("values");
		} else if (values.Length == 0) {
			throw new ArgumentException("", "values");
		}

		Console.Write(string.Format(cultureInfo, format, values));
	}

	#endregion

	#region ----- write(ConsoleColor) -----

	public static void write( ConsoleColor color, object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		write(color, Convert.ToString(value, CultureInfo.CurrentCulture));
	}

	public static void write( CultureInfo cultureInfo, ConsoleColor color, object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		write(color, Convert.ToString(value, cultureInfo));
	}

	public static void write( ConsoleColor color, string value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		ConsoleColor backupColor = ConsoleColor.Gray;
		if (!IsOutputRedirected) {
			backupColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
		}

		Console.Write(value);

		if (!IsOutputRedirected) {
			Console.ForegroundColor = backupColor;
		}
	}

	public static void write( ConsoleColor color, string format, params object[] values )
	{
		write(CultureInfo.CurrentCulture, color, format, values);
	}

	public static void write( CultureInfo cultureInfo, ConsoleColor color, string format, params object[] values )
	{
		if (format == null) {
			throw new ArgumentNullException("format");
		} else if (format.Trim().Length == 0) {
			throw new ArgumentException("", "format");
		}
		if (values == null) {
			throw new ArgumentNullException("values");
		} else if (values.Length == 0) {
			throw new ArgumentException("", "values");
		}

		ConsoleColor backupColor = ConsoleColor.Gray;
		if (!IsOutputRedirected) {
			backupColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
		}

		Console.Write(string.Format(cultureInfo, format, values));

		if (!IsOutputRedirected) {
			Console.ForegroundColor = backupColor;
		}
	}

	#endregion

	#region ----- writeln() -----

	public static void writeln()
	{
		Console.WriteLine();
	}

	public static void writeln( object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		writeln(Convert.ToString(value, CultureInfo.CurrentCulture));
	}

	public static void writeln( CultureInfo cultureInfo, object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		writeln(Convert.ToString(value, cultureInfo));
	}

	public static void writeln( string value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		if (value.Length == Console.WindowWidth) {
			Console.Write(value);
		} else {
			Console.WriteLine(value);
		}
	}

	public static void writeln( string format, params object[] values )
	{
		writeln(CultureInfo.CurrentCulture, format, values);
	}

	public static void writeln( CultureInfo cultureInfo, string format, params object[] values )
	{
		if (format == null) {
			throw new ArgumentNullException("format");
		} else if (format.Trim().Length == 0) {
			throw new ArgumentException("", "format");
		}
		if (values == null) {
			throw new ArgumentNullException("values");
		} else if (values.Length == 0) {
			throw new ArgumentException("", "values");
		}

		string val = string.Format(cultureInfo, format, values);

		if (val.Length == Console.WindowWidth) {
			Console.Write(val);
		} else {
			if (!val.EndsWith("\n") && val.IndexOf('\n') > -1) {
				string temp = val.Substring(val.LastIndexOf('\n'));
				if (temp.Length == Console.WindowWidth) {
					Console.Write(val);
				} else {
					Console.WriteLine(val);
				}
			} else {
				Console.WriteLine(val);
			}
		}
	}

	#endregion

	#region ----- writeln(ConsoleColor) -----

	public static void writeln( ConsoleColor color, object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		writeln(color, Convert.ToString(value, CultureInfo.CurrentCulture));
	}

	public static void writeln( CultureInfo cultureInfo, ConsoleColor color, object value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		writeln(color, Convert.ToString(value, cultureInfo));
	}

	public static void writeln( ConsoleColor color, string value )
	{
		if (value == null) {
			throw new ArgumentNullException("value");
		}

		ConsoleColor backupColor = ConsoleColor.Gray;
		if (!IsOutputRedirected) {
			backupColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
		}

		if (value.Length == Console.WindowWidth) {
			Console.Write(value);
		} else {
			Console.WriteLine(value);
		}

		if (!IsOutputRedirected) {
			Console.ForegroundColor = backupColor;
		}
	}

	public static void writeln( ConsoleColor color, string format, params object[] values )
	{
		writeln(CultureInfo.CurrentCulture, color, format, values);
	}

	public static void writeln( CultureInfo cultureInfo, ConsoleColor color, string format, params object[] values )
	{
		if (format == null) {
			throw new ArgumentNullException("format");
		} else if (format.Trim().Length == 0) {
			throw new ArgumentException("", "format");
		}
		if (values == null) {
			throw new ArgumentNullException("values");
		} else if (values.Length == 0) {
			throw new ArgumentException("", "values");
		}

		string val = string.Format(cultureInfo, format, values);

		ConsoleColor backupColor = ConsoleColor.Gray;
		if (!IsOutputRedirected) {
			backupColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
		}

		if (val.Length == Console.WindowWidth) {
			Console.Write(val);
		} else {
			if (!val.EndsWith("\n") && val.IndexOf('\n') > -1) {
				string temp = val.Substring(val.LastIndexOf('\n'));
				if (temp.Length == Console.WindowWidth) {
					Console.Write(val);
				} else {
					Console.WriteLine(val);
				}
			} else {
				Console.WriteLine(val);
			}
		}

		if (!IsOutputRedirected) {
			Console.ForegroundColor = backupColor;
		}
	}

	#endregion

	#region Console Redirection

	private static bool IsOutputRedirected
	{
		get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout)); }
	}

	public static bool IsInputRedirected
	{
		get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin)); }
	}

	public static bool IsErrorRedirected
	{
		get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr)); }
	}

	// P/Invoke:
	private enum FileType { Unknown, Disk, Char, Pipe };
	private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };

	[DllImport("kernel32.dll")]
	private static extern FileType GetFileType( IntPtr hdl );

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetStdHandle( StdHandle std );

	#endregion
}
