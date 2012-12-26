grammar SearchExpr;
options {
    //output=template;
	language=CSharp3;
	//ASTLabelType=CommonTree; 
}
@header {
using System;
}

@lexer::namespace { find }
@parser::namespace { find }

search
	: (match)*;
match
	: STAR {Emit(".*");}
	| QUESTION {Emit(".");}
	| MATCH {EmitEscaped($MATCH.text);};

STAR:'*';
QUESTION:'?';
MATCH:	~('*'|'?')+;

