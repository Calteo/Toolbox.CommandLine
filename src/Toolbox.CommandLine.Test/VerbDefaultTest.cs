using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Toolbox.CommandLine.Test
{
    [TestClass]
    public class VerbDefaultTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DuplicateDefaultVerbs()
        {
            Parser.Create<SimpleOption, SimpleOption>();
        }

        [TestMethod]
        public void CreateVerbWithDefault()
        {
            var cut = Parser.Create<SimpleOption, VerbListOption>();

            Assert.IsNotNull(cut.DefaultType);
            Assert.AreEqual(typeof(SimpleOption), cut.DefaultType.Type);
            Assert.AreEqual("", cut.DefaultType.Verb);
            Assert.AreEqual(1, cut.OptionTypes.Count);
            Assert.AreEqual(typeof(VerbListOption), cut.OptionTypes.First().Value.Type);
        }

        [TestMethod]
        public void ParseDefaultWithVerbs()
        {
            var cut = Parser.Create<SimpleOption, VerbListOption>();

            const string filename = "textfile.txt";
            var args = new string[] { "-f", filename };

            var result = cut.Parse(args);

            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(SimpleOption));
            Assert.AreEqual("", result.Verb);
            Assert.AreEqual(filename, ((SimpleOption)result.Option).FileName);
        }

        [TestMethod]
        public void ParseVerbWithDefault()
        {
            var cut = Parser.Create<SimpleOption, VerbListOption>();

            var args = new string[] { "list", "-a" };

            var result = cut.Parse(args);

            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbListOption));
            Assert.AreEqual("list", result.Verb);
            Assert.IsTrue(((VerbListOption)result.Option).Active);
        }



        [TestMethod]
        public void CreateWithDictionaryAndDefault()
        {            
            var verbs = new Dictionary<string, Type>
            {
                { "add", typeof(VerbAddOption)  },
                { "remove", typeof(VerbAddOption)  },
                { "", typeof(SimpleOption) }
            };

            var cut = new Parser(verbs);

            const string name = "some name";

            var args = new[] { "add", "-n", name };

            var result = cut.Parse(args);

            Assert.IsNotNull(cut.DefaultType);
            Assert.AreEqual(State.Succeeded, result.State);
            Assert.IsInstanceOfType(result.Option, typeof(VerbAddOption));
            Assert.AreEqual("add", result.Verb);
            Assert.AreEqual(name, ((VerbAddOption)result.Option).Name);
        }

        [TestMethod]
        public void RequestMixedHelp()
        {
            var cut = new Parser(typeof(VerbAddOption), typeof(VerbListOption), typeof(SimpleOption));

            var args = new string[] { "-h" };

            var result = cut.Parse(args);

            var text = result.GetHelpText();

            Assert.AreEqual(State.RequestHelp, result.State);
            Assert.IsNull(result.Option);
            Assert.IsTrue(text.Length > 0);
        }
    }
}
