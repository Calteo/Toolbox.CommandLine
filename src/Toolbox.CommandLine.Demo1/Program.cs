using System;

namespace Toolbox.CommandLine.Demo1
{
    class Program
    {
        static int Main(string[] args)
        {
            var parser = Parser.Create<DemoOptions>();
            var result = parser.Parse(args)
                .OnError(r =>
                {
                    Console.WriteLine(r.Text);
                    return -2;
                })
                .OnHelp(r => 
                {
                    Console.WriteLine(parser.GetHelpText());
                    return -1;
                })
                .On<DemoOptions>(o => 
                {
                    Console.WriteLine($"Options [{o.GetType().Name}]");
                    Console.WriteLine($"Name = '{o.Name}'");
                    if (o.Number.HasValue)
                    {
                        Console.WriteLine($"Number = '{o.Number.Value}'");
                    }
                    else
                    {
                        Console.WriteLine($"Number = not given");
                    }
                    return 0;
                }
                );

            Console.WriteLine("");
            Console.WriteLine($"return = {result.Return}");

            return result.Return;
        }
    }
}
