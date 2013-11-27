/*!
	Copyright (C) 2008-2013 Kody Brown (kody@bricksoft.com).
	
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
using System.Text;

public static class ConsolePrompts
{
	public static void PressAnyKey()
	{
		StringBuilder sb;
		string s;
		int w;
		string pad;

		s = " Press any key to exit ";
		w = (Console.WindowWidth - s.Length) / 2 - 1;

		sb = new StringBuilder();
		for (int i = 0; i < w; i++) {
			sb.Append("-");
		}
		pad = sb.ToString();

		Console.WriteLine();
		Console.WriteLine();
		Console.SetCursorPosition(0, Console.CursorTop - 2);
		Console.Write(pad + s + pad);

		Console.CursorVisible = false;
		Console.ReadKey(true);
		Console.SetCursorPosition(0, Console.CursorTop);

		sb.Clear();
		for (int i = 0; i < Console.WindowWidth - 1; i++) {
			sb.Append(" ");
		}
		Console.WriteLine(sb.ToString());
		Console.CursorVisible = true;
	}
}
