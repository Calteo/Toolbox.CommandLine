using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolbox.CommandLine
{
    /// <summary>
    /// General console program with options
    /// </summary>
    public class ConsoleProgramCore
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleProgramCore"/> class.
		/// </summary>
		/// <param name="optionTypes"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public ConsoleProgramCore(params Type[] optionTypes)
        {            
            if (optionTypes== null) throw new ArgumentNullException(nameof(optionTypes));
            if (optionTypes.Length == 0) throw new ArgumentException("No types given for options.", nameof(optionTypes));

            OptionTypes = optionTypes;
        }

        private Type[] OptionTypes { get; }
		private Dictionary<Type, Func<object, int>> OptionHandlers { get; } = new Dictionary<Type, Func<object, int>>();

		protected void AddHandler<T>(Func<T, int> handler)
		{
            if (!OptionTypes.Contains(typeof(T))) throw new ArgumentException($"Handler for type {typeof(T).FullName} is not an option type.");
            if (OptionHandlers.ContainsKey(typeof(T))) throw new ArgumentException($"Handler for type {typeof(T).FullName} already registed.", nameof(handler));

            OptionHandlers.Add(typeof(T), o => handler((T)o));
		}

		public int Run(string[] args)
        {
			try
			{
				var parser = new Parser(OptionTypes);
				var result = parser.Parse(args);

				return result
						.OnError(ExecuteError)
						.OnHelp(ExecuteHelp)
						.On(ExecuteRun)
						.Return;
			}
			catch (Exception exception)
			{
				ConsoleColor.Red.WriteLine($"exception: {exception.Message}");
				return 3;
			}
		}

		private int ExecuteRun(object option)
		{
            if (OptionHandlers.TryGetValue(option.GetType(), out var handler))
            {
                return handler(option);
            }
            return Execute(option);
        }

        protected int Execute(object option)
        {
            return 0;
        }

		protected virtual int ExecuteHelp(ParseResult result)
		{
            Console.WriteLine(result.GetHelpText(Console.WindowWidth));
            return 1;
		}

		protected virtual int ExecuteError(ParseResult result)
		{
            ConsoleColor.Red.WriteLine(result.Text);
            return 2;
		}
	}

	/// <summary>
	/// Execute a programm on a <see cref="Console"/> and handle options automatically.
	/// </summary>
	/// <remarks>
	/// The help option will create a return code of 1, an error will return 2 and an <see cref="Exception"/> will return 3.
	/// </remarks>
	public abstract class ConsoleProgram<T> : ConsoleProgramCore
	{
        public ConsoleProgram()
			: base(typeof(T))
        {
			AddHandler<T>(Execute);
        }

		protected abstract int Execute(T options);
    }
}
