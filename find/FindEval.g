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
	: e=expr { Add(e);}
	| n=other;

expr returns [Matcher value]
    :   ^(NOT a=expr) {$value = Not(a);}
    |   ^(OR a=expr b=expr) {$value= Or(a,b);}
    |   ^(AND a=expr b=expr) {$value = And(a,b);}
    |   ^(LPAREN a=expr RPAREN) {$value = a;}
    |   ^(NAME EQ v=val)   { $value = NameMatch(v,false);}
	|   ^(INAME EQ v=val)  { $value = NameMatch(v,true);}
	|   ^(REGEX EQ v=val)  { $value = RegNameMatch(v,false);}
	|   ^(IREGEX EQ v=val) { $value = RegNameMatch(v,true);}
	|   ^(SIZE EQ i=integer p=sizepostfix) {$value = Size(i,p);}
	|   ^(TYPE EQ t=type) { $value = Type(t);}
	;
other
	: ^(DEPTH EQ i=integer) { Depth(i);};

sizepostfix returns [string value]
	: s=SIZEPOSTFIX {$value=s.Text;};

integer returns [long value]
	: i=INTEGER {$value=Int64.Parse(i.Text);};

type returns [string value]
	: t=('f'|'t') {$value=t.Text;};

val returns [string value]
	: q=STRING_LITERAL { $value= q.Text.Substring(1,q.Text.Length-2);}
	| v=UNQOTED_LITERAL {$value=v.Text;};

