using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace find
{
    public partial class SearchExprParser
    {
        partial void EnterRule(string ruleName, int ruleIndex)
        {
            if (find.debug) Console.WriteLine("+" + ruleName);
        }
        partial void LeaveRule(string ruleName, int ruleIndex)
        {
            if (find.debug) Console.WriteLine("-" + ruleName);
        }

        StringBuilder sb = new StringBuilder();
        public Regex Search() 
        {
            search();
            if (find.debug) Console.WriteLine("\"" + sb.ToString() + "\"");
            return new Regex(sb.ToString());
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
