grammar gram;

@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

@parser::members
{
	protected const int EOF = Eof;
}

@lexer::members
{
	protected const int EOF = Eof;
	protected const int HIDDEN = Hidden;
}

/*
 * Parser Rules
 */

prog: statement+;

statement	: expr NL					#statement_expr
			;

expr		: if						# expr_if
			| for						# expr_for
			| '{' (expr NL)+ expr? '}'	# blockexpr
			| expr op=(MUL|DIV) expr	# MulDiv
			| expr op=(ADD|SUB) expr	# AddSub
			| expr op=(EQ|LT|GT) expr	# equality
			| INEQ expr					# inequality
			| INT						# int
			| variable					# expr_variable
			| expr '(' expr ')' 		# statement_func_call
			| expr '[' expr ']'			# list_index
			| '(' expr ')'				# parens
			| func						# expr_func
			| 'var' variable '=' expr	# statement_assignment
			| 'val' variable '=' expr	# statement_assignment_readonly
			| IDENTIFIER '=' expr		# variable_assignment
			;

variable	: IDENTIFIER
			| IDENTIFIER ':' type
			;

type		: IDENTIFIER
			;

if			: 'if' '(' expr ')' expr ('else' expr)?
			;

for			: 'for' '(' IDENTIFIER ':' expr ')' expr
			;

func		: IDENTIFIER '=>' expr		# func_literal
			;

/*
 * Lexer Rules
 */

 INT : [0-9]+;
 MUL : '*';
 DIV : '/';
 ADD : '+';
 SUB : '-';
 EQ  : '==';
 GT  : '>';
 LT  : '<';
 INEQ: '!';
 IDENTIFIER : [a-zA-Z_][a-zA-Z0-9_]*;

WS
	:	(' ' | '\r' | '\n') -> channel(HIDDEN)
	;

NL : ';'+;
