using System.ComponentModel;

namespace Toolbox.CommandLine.Test
{
    [Description("Stardard options")]
    class SimpleOption
    {
        public const string DefaultFileName = "default.txt";

        [Option("f"), DefaultValue(DefaultFileName), Description("Some filename")]
        public string FileName { get; set; }
    }
}
