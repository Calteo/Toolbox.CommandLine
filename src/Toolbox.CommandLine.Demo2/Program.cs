namespace Toolbox.CommandLine.Demo2
{
    class Program
    {
        static int Main(string[] args)
        {
            var parser = Parser.Create<DemoAddOptions, DemoRemoveOptions, DefaultOptions>();
            var result = parser.Parse(args)
                .OnError(r =>
                {
                    Console.WriteLine(r.Text);
                    return -2;
                })
                .OnHelp(r =>
                {
                    Console.WriteLine(parser.GetHelpText(r.Verb, Console.WindowWidth));
                    return -1;
                })
                .On<DemoAddOptions>(o =>
                {
                    Console.WriteLine($"Options [{o.GetType().Name}]");
                    Console.WriteLine($"Name = '{o.Name}'");
                    Console.WriteLine($"Company = '{o.Company}'");
                    Console.WriteLine($"Quiet = '{o.Quiet}'");
                    return 0;
                })
                .On<DemoRemoveOptions>(o =>
                {
                    Console.WriteLine($"Options [{o.GetType().Name}]");
                    Console.WriteLine($"Name = '{o.Name}'");
                    Console.WriteLine($"Quiet = '{o.Quiet}'");
                    return 0;
                });


            Console.WriteLine("");
            Console.WriteLine($"return = {result.Return}");

            return result.Return;
        }
    }
}
