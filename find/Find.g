grammar Find;
options {
    output=AST;
        backtrack=true;

	language=CSharp3;
	ASTLabelType=CommonTree; 
}

@lexer::namespace { find }
@parser::namespace { find }
commandline
	:expr*
	;
expr
	: paren_expr
	| or_expr
	;

paren_expr
	: LPAREN^ WS? or_expr* WS? RPAREN;
or_expr
	: and_expr (WS? OR^ WS? and_expr)*;
and_expr
	: eql_expr (WS? AND^ WS? eql_expr)*;
eql_expr
	: not_expr
	| argument;
not_expr
	:NOT^ WS? argument
	|NOT^ WS? paren_expr;

argument
	: NAME^ WS? EQ WS? value
	| INAME^ WS? EQ WS? value 
	| REGEX^ WS? EQ WS? value 
	| IREGEX^ WS? EQ WS? value  
	| TYPE^ WS? EQ WS? UNQOTED_LITERAL 
	| SIZE^ WS? EQ WS? UNQOTED_LITERAL 
	| DEPTH^ WS? EQ WS? UNQOTED_LITERAL 
	| PATH^ WS? EQ WS? value
	;

value
	: STRING_LITERAL
	| UNQOTED_LITERAL;

NAME:'-name';
INAME:'-iname';
REGEX:'-regex';
IREGEX:'-iregex';
PATH:'-path';
TYPE:'-type';
SIZE:'-size';
DEPTH:'-depth';
LPAREN: '-(' ;
RPAREN:  '-)' ;
AND:  '-AND'| '-and' | '-&&';
OR:  '-OR' | '-or' |'-||';
NOT :  '-!' | '-NOT' | '-not';
STRING_LITERAL
	:	'"' (ESC|~('"'|'\\'|'\n'|'\r'))* '"'
	;
// escape sequence -- note that this is protected; it can only be called
//   from another lexer rule -- it will not ever directly return a token to
//   the parser
// There are various ambiguities hushed in this rule.  The optional
// '0'...'9' digit matches should be matched here rather than letting
// them go back to STRING_LITERAL to be matched.  ANTLR does the
// right thing by matching immediately; hence, it's ok to shut off
// the FOLLOW ambig warnings.
fragment
ESC
	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('u')+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
		|	'0'..'3'
			(
			:	'0'..'7'
				(
				:	'0'..'7'
				)?
			)?
		|	'4'..'7'
			(
			:	'0'..'7'
			)?
		)
	;


// hexadecimal digit (again, note it's protected!)
fragment
HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;
UNQOTED_LITERAL
	:	~(' '|'-'|'=')*;
EQ : '=';
WS : (' ' | '\t' | '\r' | '\n')+ {$channel = Hidden ;} ;
