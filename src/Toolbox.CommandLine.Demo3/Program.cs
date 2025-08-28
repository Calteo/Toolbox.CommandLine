using System.ComponentModel;

namespace Toolbox.CommandLine.Demo3
{
	internal class Program : ConsoleProgramCore
	{
		static int Main(string[] args)
		{
			return new Program().Run(args);
		}

        public Program()
			: base(typeof(DemoOptions))
        {
			AddHandler<DemoOptions>(Execute);
        }

		private int Execute(DemoOptions options)
		{
			Console.WriteLine($"execute {options.Sleep}");
			Thread.Sleep(options.Sleep);

			return 0;
		}
	}
}
