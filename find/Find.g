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
	| TYPE^ WS? EQ WS? ('f'|'d') 
	| SIZE^ WS? EQ WS? INTEGER WS? SIZEPOSTFIX? 
	| DEPTH^ WS? EQ WS? INTEGER ;

value
	: STRING_LITERAL
	| UNQOTED_LITERAL;

NAME:'-name';
INAME:'-iname';
REGEX:'-regex';
IREGEX:'-iregex';
TYPE:'-type';
SIZE:'-size';
DEPTH:'-depth';
LPAREN: '-(' ;
RPAREN:  '-)' ;
AND:  '-AND'| '-and' | '-&&';
OR:  '-OR' | '-or' |'-||';
NOT :  '-!' | '-NOT' | '-not';
INTEGER: ('0'..'9')+;
SIZEPOSTFIX: ('c'|'w'|'k'|'M'|'G'|'b');
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
protected
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
				//options {
				//	warnWhenFollowAmbig = false;
				//}
			:	'0'..'7'
				(
				//	options {
				//		warnWhenFollowAmbig = false;
				//	}
				:	'0'..'7'
				)?
			)?
		|	'4'..'7'
			(
				//options {
				//	warnWhenFollowAmbig = false;
				//}
			:	'0'..'7'
			)?
		)
	;


// hexadecimal digit (again, note it's protected!)
protected
HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;
UNQOTED_LITERAL
	:	~(' '|'-'|'=')*;
EQ : '=';
WS : (' ' | '\t' | '\r' | '\n')+ {$channel = Hidden ;} ;
