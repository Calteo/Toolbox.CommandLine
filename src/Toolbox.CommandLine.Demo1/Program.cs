using System;

namespace Toolbox.CommandLine.Demo1
{
    class Program
    {
        static int Main(string[] args)
        {
            var rc =  ConsoleProgram.Run<DemoOptions>(args, Excecute);

            Console.WriteLine("-- programm ended ---");
            Console.WriteLine($"return = {rc}");

            return rc;
        }

        private static int Excecute(DemoOptions options)
        {
            Console.WriteLine($"Options [{options.GetType().Name}]");
            Console.WriteLine($"Name = '{options.Name}'");
            if (options.Number.HasValue)
            {
                Console.WriteLine($"Number = '{options.Number.Value}'");
            }
            else
            {
                Console.WriteLine($"Number = not given");
            }
            return 0;
        }
    }
}
