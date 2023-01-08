using System.ComponentModel;

namespace Toolbox.CommandLine.Test
{
    [Verb("list")]
    [Description("some option for lists")]
    class VerbListOption
    {
        [Option("a")]
        public bool Active { get; set; }
    }
}
