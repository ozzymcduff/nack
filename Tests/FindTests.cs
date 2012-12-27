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
            return FindEval.Parse(str);
        }

        [Test]
        public void Regex_should_match() 
        {
            var r = Parse("-regex=find.*");
            Assert.That(r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(!r.filesearch("./test/gind.exe"), "gind.exe");
        }
        [Test]
        public void Path_regex_should_match()
        {
            var r = Parse("-regex=find");
            Assert.That(r.pathsearch("./test/find/"), "find/");
            Assert.That(!r.pathsearch("./test/gind/"), "gind/");
            Assert.That(r.pathsearch(".\\test\\find\\"), "find\\");
            Assert.That(!r.pathsearch(".\\test\\gind\\"), "gind\\");
            Assert.That(r.pathsearch("./test/find"), "find");
            Assert.That(!r.pathsearch("./test/gind"), "gind");
        }
        [Test]
        public void Path_should_match()
        {
            var r = Parse("-path=find");
            Assert.That(r.pathsearch("./test/find/"), "path find/");
            Assert.That(!r.pathsearch("./test/gind/"), "path gind/");
            Assert.That(r.filesearch("./test/find/find"), "file find");
            Assert.That(!r.filesearch("./test/gind/find"), "file gind");
        }
        [Test]
        public void Not_regex_should_not_match()
        {
            var r = Parse("-not -regex=find.*");
            Assert.That(!r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(r.filesearch("./test/gind.exe"), "gind.exe");
        }

        [Test]
        public void And_regex_should_match()
        {
            var r = Parse("-regex=find.* -and -regex=.*.exe");
            Assert.That(r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(!r.filesearch("./test/gind.exe"), "gind.exe");
            Assert.That(!r.filesearch("./test/find.com"), "find.com");
        }
        [Test]
        public void Implicit_and_regex_should_match()
        {
            var r = Parse("-regex=find.* -regex=.*.exe");
            Assert.That(r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(!r.filesearch("./test/gind.exe"), "gind.exe");
            Assert.That(!r.filesearch("./test/find.com"), "find.com");
        }

        [Test]
        public void Or_regex_should_match()
        {
            var r = Parse("-regex=find.* -or -regex=.*.exe");
            Assert.That(r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(r.filesearch("./test/gind.exe"), "gind.exe");
            Assert.That(r.filesearch("./test/find.com"), "find.com");
            Assert.That(!r.filesearch("./test/gind.com"), "gind.com");
        }
        [Test]
        public void Parens_Or_regex_should_match()
        {
            var r = Parse("-( -regex=find.* -or -regex=.*.exe -) ");
            Assert.That(r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(r.filesearch("./test/gind.exe"), "gind.exe");
            Assert.That(r.filesearch("./test/find.com"), "find.com");
            Assert.That(!r.filesearch("./test/gind.com"), "gind.com");
        }

        [Test]
        public void Not_parens_Or_regex_should_match()
        {
            var r = Parse("-not -( -regex=find.* -or -regex=.*.exe -)");
            Assert.That(!r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(!r.filesearch("./test/gind.exe"), "gind.exe");
            Assert.That(!r.filesearch("./test/find.com"), "find.com");
            Assert.That(r.filesearch("./test/gind.com"), "gind.com");
        }

        [Test]
        public void Not_parens_Or_regex_should_match_and_know_depth()
        {
            var r = Parse("-not -( -regex=find.* -or -regex=.*.exe -) -depth=10");
            Assert.That(r.maxdepth, Is.EqualTo(10));
            Assert.That(!r.filesearch("./test/find.exe"), "find.exe");
            Assert.That(!r.filesearch("./test/gind.exe"), "gind.exe");
            Assert.That(!r.filesearch("./test/find.com"), "find.com");
            Assert.That(r.filesearch("./test/gind.com"), "gind.com");
        }

    }
}
