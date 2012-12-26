tree grammar FindEval;
options {
    tokenVocab=Find;
	language=CSharp3;
	ASTLabelType=CommonTree;
}
@namespace { find }
@header {
using System;
using Matcher=System.Func<string,find.find.Type,bool>;
}

commandline
	: expression*;

expression
	: e=expr { Add(e);};

expr returns [Matcher value]
    :   ^(NOT a=expr) {$value = Not(a);}
    |   ^(OR a=expr b=expr) {$value= Or(a,b);}
    |   ^(AND a=expr b=expr) {$value = And(a,b);}
    |   ^(LPAREN a=expr RPAREN) {$value = a;}
    |   ^(NAME EQ v=val)   { $value = NameMatch(v,false);}
	|   ^(INAME EQ v=val)  { $value = NameMatch(v,true);}
	|   ^(REGEX EQ v=val)  { $value = RegNameMatch(v,false);}
	|   ^(IREGEX EQ v=val) { $value = RegNameMatch(v,true);}
	;

val returns [string value]
	: q=STRING_LITERAL { $value= q.Text.Substring(1,q.Text.Length-2);}
	| v=UNQOTED_LITERAL {$value=v.Text;};

