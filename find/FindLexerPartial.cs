using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace find
{
    public partial class FindLexer
    {
        partial void EnterRule(string ruleName, int ruleIndex) 
        {
            if (find.debug) Console.WriteLine("l+" + ruleName);
        }
        partial void LeaveRule(string ruleName, int ruleIndex) 
        {
            if (find.debug) Console.WriteLine("l-" + ruleName);
        }
        protected override void DebugRecognitionException(Antlr.Runtime.RecognitionException ex)
        {
            Console.WriteLine("L:DebugRecognitionException");
            Console.WriteLine(ex);
      
            base.DebugRecognitionException(ex);
        }
        public override void ReportError(Antlr.Runtime.RecognitionException e)
        {
            Console.WriteLine("L:");
            Console.WriteLine(e);
            base.ReportError(e);
        }
    }
}
