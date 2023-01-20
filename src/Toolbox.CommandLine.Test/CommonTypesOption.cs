using System;
using System.ComponentModel;

namespace Toolbox.CommandLine.Test
{
    class CommonTypesOption
    {
        [Option("text"), Position(0)]
        public string Text { get; set; }

        [Option("switch")]
        public bool Switch { get; set; }

        [Option("number"), Position(1)]
        public int Number { get; set; }

        [Option("decimal")]
        public decimal Decimal { get; set; }

        [Option("date"),Position(2)]
        public DateTime Date { get; set; }

        [Option("timespan")]
        [DefaultValue("30")]
        public TimeSpan TimeSpan { get; set; }

        [Option("now")]
        public DateTime Now { get; set; }

        [Option("nullnumber")]
        public int? NullNumber { get; set; }

        [Option("long")]
        [DefaultValue(long.MaxValue)]
        public long LongNumber { get; set; }

        [Option("enum")]
        [DefaultValue(Choice.None)]
        [Description("you have to make a choice here")]
        public Choice Choice { get; set; }
    }

    [Description("some choice")]
    enum Choice
    {
        [Description("take none")]
        None,
        [Description("take one")]
        One,
        [Description("take some more")]
        Many,
        [Description("take all of them")]
        All
    }
}
