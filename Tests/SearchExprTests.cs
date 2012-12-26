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
            var s = new MemoryStream();
            var wr = new StreamWriter(s);
            wr.Write(str);
            wr.Flush();
            s.Position = 0;
            var input = new ANTLRInputStream(s);
            // Create an ExprLexer that feeds from that stream
            var lexer = new SearchExprLexer(input);
            // Create a stream of tokens fed by the lexer
            var tokens = new CommonTokenStream(lexer);
            // Create a parser that feeds off the token stream
            var parser = new SearchExprParser(tokens);
            return parser.Search();
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
