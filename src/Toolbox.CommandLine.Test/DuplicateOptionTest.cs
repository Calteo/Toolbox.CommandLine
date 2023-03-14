using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Toolbox.CommandLine.Test
{
    [TestClass]
    public class DuplicateOptionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowOnDuplicateOption()
        {
            var _ = new Parser(typeof(DuplicateOption));

            Assert.Fail("no exception");
        }
    }
}
