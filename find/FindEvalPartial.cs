using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matcher = System.Func<string, find.find.Type, bool>;
using System.Text.RegularExpressions;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace find
{
    public partial class FindEval
    {
        public static RetVal Parse(string str) 
        {
            using(var s = new MemoryStream())
            using (var wr = new StreamWriter(s))
            {
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
        }

        public class RetVal
        {
            public string path;
            public Matcher onsearch;
            public long? maxdepth;
            public string searchexpr;
            public bool filesearch(string s) 
            {
                return onsearch(s, find.Type.File);
            }
        }
        private string searchExpr = null;
        private IList<Matcher> matchers = new List<Matcher>();
        private long? maxdepth;
        private string path;
        private bool ReturnFalse(string s,find.Type t) { return false; }
        public RetVal CommandLine()
        {
            this.commandline();
            return new RetVal
            {
                onsearch = Reduce<Matcher, Matcher>(matchers.Skip(1), matchers.FirstOrDefault() ?? ReturnFalse, And),
                searchexpr = this.searchExpr,
                maxdepth = maxdepth,
                path = path
            };
        }
        private Memo Reduce<T, Memo>(IEnumerable<T> enumerable, Memo memo, Func<Memo, T, Memo> reduce)
        {
            foreach (var item in enumerable)
            {
                memo = reduce(memo, item);
            }
            return memo;
        }
        void Add(Matcher m)
        {
            if (find.debug) Console.WriteLine("add");
            matchers.Add(m);
        }
        Matcher File(Func<string, bool> match) 
        {
            return (string s, find.Type t) => t.IsFile()&&match(s);
        }
        Matcher Directory(Func<string, bool> match)
        {
            return (string s, find.Type t) => t.IsDirectory()&&match(s);
        }
        Matcher NameMatch(string value, bool caseInsensitive)
        {
            //searchExpr = value;
            var m = SearchExprParser.Parse(value, caseInsensitive: caseInsensitive);
            return File((string s) => m.IsMatch(FileName(s)));
        }
        Matcher RegNameMatch(string value, bool caseInsensitive)
        {
            if (find.debug) Console.WriteLine("regex");

            var options = RegexOptions.Compiled;
            if (caseInsensitive)
                options |= RegexOptions.IgnoreCase;
            var regex = new Regex(value, options);
            return File((string s) => regex.IsMatch(FileName(s)));
        }

        private string FileName(string s)
        {
            return System.IO.Path.GetFileName(s);
        }
        Matcher And(Matcher a, Matcher b)
        {
            if (find.debug) Console.WriteLine("and");
            Matcher onsearch = (string s, find.Type t) => a(s,t) && b(s,t);
            return onsearch;
        }
        Matcher Or(Matcher a, Matcher b)
        {
            if (find.debug) Console.WriteLine("or");
            Matcher onsearch = (string s, find.Type t) => a(s,t) || b(s,t);
            return onsearch;
        }
        Matcher Not(Matcher a)
        {
            if (find.debug) Console.WriteLine("not");
            Matcher onsearch = (string s, find.Type t) => !a(s,t);
            return onsearch;
        }
        Matcher Type(string tp)
        {
            if (find.debug) Console.WriteLine("type");
            find.Type type;
            switch (tp)
            {
                case "f":
                    type = find.Type.File;
                    break;
                case "d":
                    type = find.Type.Directory;
                    break;
                default:
                    throw new NotImplementedException(tp);
            }
            Matcher onsearch = (string s, find.Type t) => t==type;
            return onsearch;
        }
        void Depth(long d)
        {
            if (find.debug) Console.WriteLine("depth");
            maxdepth = d;
        }
        private long Factor(string postfix) 
        {
            int factor = 512;
            switch (postfix.SingleOrDefault())
            {
                case 'c'://'c'    for bytes
                    factor = 1; break;
                case 'w'://'w'    for two-byte words
                    factor = 2; break;
                case 'k'://'k'    for Kilobytes (units of 1024 bytes)
                    factor = 1024; break;
                case 'M'://'M'    for Megabytes (units of 1048576 bytes)
                    factor = 1048576; break;
                case 'G'://'G'    for Gigabytes (units of 1073741824 bytes)
                    factor = 1073741824; break;
                case 'b'://'b'    for 512-byte blocks (this is the default if no suffix  is used)
                    factor = 512; break;
            }
            return factor;
        }
        Matcher Size(long size,string postfix)
        {
            if (find.debug) Console.WriteLine("size");
            var sizev = size*Factor(postfix);
            return File((string s) => new FileInfo(s).Length > sizev);
        }
    }
}
