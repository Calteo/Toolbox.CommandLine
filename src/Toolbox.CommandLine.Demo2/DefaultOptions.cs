using System.ComponentModel;

namespace Toolbox.CommandLine.Demo2
{
    [Description("Default options")]
    internal class DefaultOptions
    {
        [Option("action"), Position(0), Description("action to perform")]
        public string? Action { get; set; }
    }
}
