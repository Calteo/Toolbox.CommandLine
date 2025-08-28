using System;
using System.Net.Http.Headers;

namespace Toolbox.CommandLine.Demo1
{
    class Program : ConsoleProgram<DemoOptions>
    {
        static int Main(string[] args)
        {
            var rc =  new Program().Run(args);

            Console.WriteLine("-- programm ended ---");
            Console.WriteLine($"return = {rc}");

            return rc;
        }

		protected override int Execute(DemoOptions options)
		{
			ConsoleColor.Green.WriteLine($"Options [{options.GetType().Name}]");
			Console.WriteLine($"Name = '{options.Name}'");
			if (options.Number.HasValue)
			{
				Console.WriteLine($"Number = '{options.Number.Value}'");
			}
			else
			{
				ConsoleColor.Yellow.WriteLine("Number = not given");
			}
			return 0;
		}
	}
}
