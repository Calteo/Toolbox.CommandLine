using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Toolbox.CommandLine.Test
{
    [TestClass]
    public class VerbTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DuplicateVerbs()
        {
            Parser.Create<VerbAddOption, VerbAddOption>();
        }

        [TestMethod]
        public void ParseAddVerb()
        {
            var cut = Parser.Create<VerbAddOption, VerbListOption>();

            const string name = "some name";

            var args = new[] { "add", "-n", name };

            var result = cut.Parse(args);

            Assert.IsNull(cut.DefaultType);
            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbAddOption));
            Assert.AreEqual(name, ((VerbAddOption)result.Option).Name);
        }

        [TestMethod]
        public void ParseListVerb()
        {
            var cut = Parser.Create<VerbAddOption, VerbListOption>();

            var args = new[] { "list", "-a" };

            var result = cut.Parse(args);

            Assert.IsNull(cut.DefaultType);
            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbListOption));
            Assert.IsTrue(((VerbListOption)result.Option).Active);
        }

        [TestMethod]
        public void RequestHelpOnVerbs()
        {
            var cut = Parser.Create<VerbAddOption, VerbListOption>();

            var args = new string[] { "add", "-h" };

            var result = cut.Parse(args);

            Assert.AreEqual(State.RequestHelp, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbAddOption));
        }

        [TestMethod]
        public void RequestHelp()
        {
            var cut = Parser.Create<VerbAddOption, VerbListOption>();

            var args = new string[] { "-h" };

            var result = cut.Parse(args);

            Assert.AreEqual(State.RequestHelp, result.State);
            Assert.IsNull(result.Option);
        }

        [TestMethod]
        public void ParseVerbsSameOptionClass()
        {
            var verbs = new Dictionary<string, Type>
            {
                { "add", typeof(VerbAddOption)  },
                { "remove", typeof(VerbAddOption)  },
            };
            var cut = new Parser(verbs);

            const string name = "some name";

            var args = new[] { "add", "-n", name };

            var result = cut.Parse(args);

            Assert.IsNull(cut.DefaultType);
            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbAddOption));
            Assert.AreEqual("add", result.Verb);
            Assert.AreEqual(name, ((VerbAddOption)result.Option).Name);
        }

        [TestMethod]
        public void ParseVerbsSameOptionClassSecondClass()
        {
            var verbs = new Dictionary<string, Type>
            {
                { "add", typeof(VerbAddOption)  },
                { "remove", typeof(VerbAddOption)  },
            };
            var cut = new Parser(verbs);

            const string name = "some name";

            var args = new[] { "remove", "-n", name };

            var result = cut.Parse(args);

            Assert.IsNull(cut.DefaultType);
            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbAddOption));
            Assert.AreEqual("remove", result.Verb);
            Assert.AreEqual(name, ((VerbAddOption)result.Option).Name);
        }

    }
}
