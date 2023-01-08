using System.ComponentModel;

namespace Toolbox.CommandLine.Demo2
{
    [Verb("add")]
    [Description("Adds a person to the club")]
    class DemoAddOptions : DemoOptions
    {
        [Option("company"), Position(1), DefaultValue("myCompany")]
        [Description("The company of a person.")]
        public string? Company { get; set; }
    }
}
