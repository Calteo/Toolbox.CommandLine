using System;

namespace Toolbox.CommandLine
{
	/// <summary>
	/// Extensions to the <see cref="ConsoleColor"/>.
	/// </summary>
	public static class ConsoleColorExtension
	{
		/// <summary>
		/// Write a line in the color
		/// </summary>
		/// <param name="color"></param>
		/// <param name="text"></param>
		/// <remarks>
		/// The colors are reset after the call. 
		/// </remarks>
		/// <see cref="Console.ResetColor"/>
		public static void WriteLine(this ConsoleColor color, string text)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ResetColor();
		}
	}
}
