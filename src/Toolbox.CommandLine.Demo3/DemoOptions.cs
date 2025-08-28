using System.ComponentModel;

namespace Toolbox.CommandLine.Demo3
{
	internal class DemoOptions
	{
		[Option("sleep"), Mandatory, Position(0), DefaultValue(1000), Description("time to wait (in ms)")]
		public int Sleep { get; set; }
	}
}
