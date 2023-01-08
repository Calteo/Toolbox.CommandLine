# Mutiple options

Multiple options can be used by the <xref:Toolbox.CommandLine.VerbAttribute>.

The verb decides which option type is to be used, so it must be the first argument on the command line.

```
    class DemoOptions
    {
        [Option("name"), Mandatory, Position(0)]
        [Description("The name of a person.")]
        public string Name { get; set; }

        [Option("quiet")]
        [Description("Turns off most of the output")]
        public bool Quiet { get; set; }
    }

    [Verb("add")]
    [Description("Adds a person to the club")]
    class DemoAddOptions : DemoOptions
    {
        [Option("company"), Position(1), DefaultValue("myCompany")]
        [Description("The company of a person.")]
        public string Company { get; set; }
    }

    [Verb("remove")]
    [Description("Removes a person from the club.")]
    class DemoRemoveOptions : DemoOptions
    {
    }
```

Here we have to option types, sharing some common properties.

```
some.exe add "Hugo"
```
using the `DemoAddOptions`. Note that `Name` is selected by its position and `Company` gets 
the default value `"myCompany"`.

```
some.exe remove -name "Hugo" -quiet
```
using the `DemoRemoveOptions`. 

The program might look like this
```
    static int Main(string[] args)
    {
        var parser = Parser.Create<DemoAddOptions, DemoRemoveOptions>();
        var result = parser.Parse(args)
            .OnError(r =>
            {
                // too bad, an error occured
                Console.WriteLine(r.Text);
                return -2;
            })
            .OnHelp(r =>
            {
                // ok - output some help
                Console.WriteLine(parser.GetHelpText(r.Verb));
                return -1;
            })
            .On<DemoAddOptions>(o =>
            {
                // the add verb was given
                Console.WriteLine($"Options [{o.GetType().Name}]");
                Console.WriteLine($"Name = '{o.Name}'");
                Console.WriteLine($"Company = '{o.Company}'");
                Console.WriteLine($"Quiet = '{o.Quiet}'");
                return 0;
            })
            .On<DemoRemoveOptions>(o =>
            {
                // the remove verb was given
                Console.WriteLine($"Options [{o.GetType().Name}]");
                Console.WriteLine($"Name = '{o.Name}'");
                Console.WriteLine($"Quiet = '{o.Quiet}'");
                return 0;
            });

        Console.WriteLine("");
        Console.WriteLine($"return = {result.Return}");

        return result.Return;
    }
```

## Dafault options with verbs

One option can obmit the verb and will be used as a default option if no verb is givem.
Consider adding to the above example an option class as a default.
```
    class DefaultOptions
    {
        [Option("action"), Mandatory, Position(0)]
        [Description("The name of an action to perform.")]
        public string Action { get; set; }
    }
```
Using a parser having all three option classes wil allow the following arguments:
- `some.exe add Name Company`
   will use the `DemoAddOptions`.
- `some.exe remove Name`
   will usee the `DemoRemoveOptions`.
- `some.exe -action Action`
   will use the `DefaultOptions`, since no verb is given.
- `some.exe Action`
   will use the `DefaultOptions`, because it defines a positional argument at position 0 and `Action` 
   is not a valid verb.
- `some.exe -action add`
   will use the `DefaultOptions` with the first argument value of `add`. The command `some.exe add` 
   would select the `DefaultAddOption`, since it contains a valid verb.

