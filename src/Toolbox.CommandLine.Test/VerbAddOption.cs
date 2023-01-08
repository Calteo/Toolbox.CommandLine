using System.ComponentModel;

namespace Toolbox.CommandLine.Test
{
    [Verb("add")]
    [Description("some option for adding")]
    class VerbAddOption
    {
        [Option("n")]
        public string Name { get; set; }

    }
}
