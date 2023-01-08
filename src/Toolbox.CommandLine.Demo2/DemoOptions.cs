using System.ComponentModel;

namespace Toolbox.CommandLine.Demo2
{
    class DemoOptions
    {
        [Option("name"), Mandatory, Position(0)]
        [Description("The name of a person.")]
        public string? Name { get; set; }

        [Option("quiet")]
        [Description("Turns off most of the output")]
        public bool Quiet { get; set; }
    }
}
