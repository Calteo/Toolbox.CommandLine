﻿using System;

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

        [Option("now")]
        public DateTime Now { get; set; }

        [Option("nullnumber")]
        public int? NullNumber { get; set; }


    }
}
