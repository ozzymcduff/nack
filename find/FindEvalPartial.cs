using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matcher = System.Func<string, find.find.Type, bool>;
using System.Text.RegularExpressions;

namespace find
{
    public partial class FindEval
    {
        public class RetVal
        {
            public string path;
            public Matcher onsearch;
            public int? maxdepth;
            public string searchexpr;
        }
        private string searchExpr=null;
        private IList<Matcher> matchers = new List<Matcher>();
        private int? maxdepth;
        private string path;
        public RetVal CommandLine() 
        {
            this.commandline();
            return new RetVal { 
                onsearch = Reduce<Matcher, Matcher>(matchers.Skip(1), matchers.FirstOrDefault(), And),
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
        Matcher NameMatch(string value, bool caseInsensitive)
        {
            searchExpr = value;
            throw new NotImplementedException();
            //Matcher onsearch = (string s, find.Type t) => true;
            //return onsearch;
        }
        Matcher RegNameMatch(string value, bool caseInsensitive)
        {
            if (find.debug) Console.WriteLine("regex");

            var options = RegexOptions.Compiled;
            if (caseInsensitive)
                options |= RegexOptions.IgnoreCase;
            var regex = new Regex(value, options);
            Matcher onsearch = (string s, find.Type t) => regex.IsMatch(s);
            return onsearch;
        }
        Matcher And(Matcher a, Matcher b)
        {
            if (find.debug) Console.WriteLine("and");
            Matcher onsearch = (string s, find.Type t) => a(s, t) && b(s, t);
            return onsearch;
        }
        Matcher Or(Matcher a, Matcher b)
        {
            if (find.debug) Console.WriteLine("or");
            Matcher onsearch = (string s, find.Type t) => a(s, t) || b(s, t);
            return onsearch;
        }
        Matcher Not(Matcher a)
        {
            if (find.debug) Console.WriteLine("not");
            Matcher onsearch = (string s, find.Type t) => !a(s, t);
            return onsearch;
        }

    }
}
