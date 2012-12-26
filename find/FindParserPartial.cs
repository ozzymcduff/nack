using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace find
{
    public partial class FindParser
    {
        //partial void EnterRule(string ruleName, int ruleIndex) 
        //{
        //    //if (find.debug) Console.WriteLine("+" + ruleName);
        //}
        //partial void LeaveRule(string ruleName, int ruleIndex) 
        //{
        //    //if (find.debug) Console.WriteLine("-" + ruleName);
        //}

        public Antlr.Runtime.Tree.CommonTree CommandLine()
        {
            var cli = this.commandline();
            if (find.debug)
            {
                //Console.WriteLine(cli.Start.Text+" "+cli.Start.Type);

                //Console.WriteLine(cli.Stop.Text + " " + cli.Stop.Type);
            }
            if (find.debug && null!=cli && null!=cli.Tree)Console.WriteLine(cli.Tree.ToStringTree());
            return cli.Tree;
        }
        public override void ReportError(Antlr.Runtime.RecognitionException e)
        {
            if (find.debug)
            {
                Console.WriteLine("ReportError");
                Console.WriteLine(e);
            }
            base.ReportError(e);
        }
        public override void Recover(Antlr.Runtime.IIntStream input, Antlr.Runtime.RecognitionException re)
        {
            if (find.debug)
            {
                Console.WriteLine("Recover");
                Console.WriteLine(re);
            }
            base.Recover(input, re);
        }
        public override object RecoverFromMismatchedSet(Antlr.Runtime.IIntStream input, Antlr.Runtime.RecognitionException e, Antlr.Runtime.BitSet follow)
        {
            if (find.debug)
            {
                Console.WriteLine("RecoverFromMismatchedSet");
                Console.WriteLine(e);
            }
            return base.RecoverFromMismatchedSet(input, e, follow);
        }
        protected override object RecoverFromMismatchedToken(Antlr.Runtime.IIntStream input, int ttype, Antlr.Runtime.BitSet follow)
        {
            if (find.debug)
            {
                Console.WriteLine("RecoverFromMismatchedToken");
            }
            return base.RecoverFromMismatchedToken(input, ttype, follow);
        }
        protected override void DebugEnterRule(string grammarFileName, string ruleName)
        {
            if (find.debug)
                Console.WriteLine("  +" + ruleName);
            
            base.DebugEnterRule(grammarFileName, ruleName);
        }
        protected override void DebugExitRule(string grammarFileName, string ruleName)
        {
            if (find.debug)
                Console.WriteLine("  -" + ruleName);
            base.DebugExitRule(grammarFileName, ruleName);
        }
        protected override void DebugRecognitionException(Antlr.Runtime.RecognitionException ex)
        {
            if (find.debug)
               Console.WriteLine(ex);
            
            base.DebugRecognitionException(ex);
        }

    }
}
