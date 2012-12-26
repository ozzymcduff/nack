using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Antlr.Runtime;
using find;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public partial class SearchExprTests
    {
        private Regex Parse(string str)
        {
            return SearchExprParser.Parse(str);
        }
        [Test]
        public void Star() 
        {
            var r = Parse("*.test");
            Assert.That(r.Match("something.test").Success, "something.test");
            Assert.That(!r.Match("something.etest").Success, "something.etest");
        }
        [Test]
        public void Question()
        {
            var r = Parse("?.test");
            Assert.That(r.Match("s.test").Success, "s.test");
            Assert.That(!r.Match("s2.etest").Success, "s2.etest");
            Assert.That(!r.Match("s.etest").Success, "s.etest");
        }

    }
}
