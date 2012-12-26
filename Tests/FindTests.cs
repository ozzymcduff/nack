using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Antlr.Runtime;
using find;
using Antlr.Runtime.Tree;

namespace Tests
{
    [TestFixture] public class FindTests
    {
        private FindEval.RetVal Parse(string str) 
        {
            var s = new MemoryStream();
            var wr = new StreamWriter(s);
            wr.Write(str);
            wr.Flush();
            s.Position = 0;
            ANTLRInputStream input = new ANTLRInputStream(s);
            // Create an ExprLexer that feeds from that stream
            FindLexer lexer = new FindLexer(input);
            // Create a stream of tokens fed by the lexer
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            // Create a parser that feeds off the token stream
            FindParser parser = new FindParser(tokens);
            // Begin parsing at rule prog, get return value structure
            var c = parser.CommandLine();
            var cstream = new CommonTreeNodeStream(c);
            var eval = new FindEval(cstream);
         
            var cli = eval.CommandLine();
            return cli;
        }
        [Test]
        public void Regex_should_match() 
        {
            var r = Parse("-regex=find.*");
            Assert.That(r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(!r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
        }
        [Test]
        public void Not_regex_should_not_match()
        {
            var r = Parse("-not -regex=find.*");
            Assert.That(!r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
        }

        [Test]
        public void And_regex_should_match()
        {
            var r = Parse("-regex=find.* -and -regex=.*.exe");
            Assert.That(r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(!r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
            Assert.That(!r.onsearch("find.com", find.find.Type.File), "find.com");
        }
        [Test]
        public void Implicit_and_regex_should_match()
        {
            var r = Parse("-regex=find.* -regex=.*.exe");
            Assert.That(r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(!r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
            Assert.That(!r.onsearch("find.com", find.find.Type.File), "find.com");
        }

        [Test]
        public void Or_regex_should_match()
        {
            var r = Parse("-regex=find.* -or -regex=.*.exe");
            Assert.That(r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
            Assert.That(r.onsearch("find.com", find.find.Type.File), "find.com");
            Assert.That(!r.onsearch("gind.com", find.find.Type.File), "gind.com");
        }
        [Test]
        public void Parens_Or_regex_should_match()
        {
            var r = Parse("-( -regex=find.* -or -regex=.*.exe -) ");
            Assert.That(r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
            Assert.That(r.onsearch("find.com", find.find.Type.File), "find.com");
            Assert.That(!r.onsearch("gind.com", find.find.Type.File), "gind.com");
        }

        [Test]
        public void Not_parens_Or_regex_should_match()
        {
            var r = Parse("-not -( -regex=find.* -or -regex=.*.exe -)");
            Assert.That(!r.onsearch("find.exe", find.find.Type.File), "find.exe");
            Assert.That(!r.onsearch("gind.exe", find.find.Type.File), "gind.exe");
            Assert.That(!r.onsearch("find.com", find.find.Type.File), "find.com");
            Assert.That(r.onsearch("gind.com", find.find.Type.File), "gind.com");
        }
    }
}
