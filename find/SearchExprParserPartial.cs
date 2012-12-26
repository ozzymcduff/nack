using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Antlr.Runtime;

namespace find
{
    public partial class SearchExprParser
    {
        public static Regex Parse(string str, bool caseInsensitive=false)
        {
            using (var s = new MemoryStream())
            using (var wr = new StreamWriter(s))
            {
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
                return parser.Search(caseInsensitive);
            }
        }

        partial void EnterRule(string ruleName, int ruleIndex)
        {
            if (find.debug) Console.WriteLine("+" + ruleName);
        }
        partial void LeaveRule(string ruleName, int ruleIndex)
        {
            if (find.debug) Console.WriteLine("-" + ruleName);
        }

        StringBuilder sb = new StringBuilder();
        public Regex Search(bool caseInsensitive) 
        {
            search();
            var options = RegexOptions.Compiled;
            if (caseInsensitive)
                options |= RegexOptions.IgnoreCase;
            if (find.debug) Console.WriteLine("\"" + sb.ToString() + "\"");
            return new Regex(sb.ToString(), options);
        }
        public void Emit(String val) 
        {
            sb.Append(val);
        }
        public void EmitEscaped(String val) 
        {
            sb.Append(Regex.Escape(val));
        }
    }
}
