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
			| while						# expr_while
			| '{' (expr NL)* expr? '}'	# blockexpr
			| expr op=(MUL|DIV) expr	# MulDiv
			| expr op=(ADD|SUB) expr	# AddSub
			| expr op=(EQ|LT|GT) expr	# equality
			| expr op=(OR|AND) expr		# logicaloperator
			| INEQ expr					# inequality
			| INT						# int
			| func						# expr_func
			| variable					# expr_variable
			| expr '('? expr ')'? 		# statement_func_call
			| expr '[' expr ']'			# list_index
			| '(' expr ')'				# parens
			| type						# expr_type
			| 'var' binding '=' expr	# statement_assignment
			| 'val' binding '=' expr	# statement_assignment_readonly
			| IDENTIFIER '=' expr		# variable_assignment
			;

variable	: IDENTIFIER
			| IDENTIFIER ':' type
			;

binding		: variable						#binding_single
			| '{' (binding NL)* binding? '}'#binding_multiple
			;

type		: IDENTIFIER					#rawtype
			| type '=>' type				#functype
			| type '<' expr '>'				#predtype
			| '{' (type NL)* type? '}'		#listtype
			;

if			: 'if' '(' expr ')' expr ('else' expr)?
			;

for			: 'for' '(' binding ':' expr ')' expr
			;

while		: 'while' '(' expr ')' expr
			;

func		: binding '=>' expr		# func_literal
			;

/*
 * Lexer Rules
 */

 INT : '-'?[0-9]+;
 MUL : '*';
 DIV : '/';
 ADD : '+';
 SUB : '-';
 EQ  : '==';
 GT  : '>';
 LT  : '<';
 INEQ: '!';
 OR  : '||';
 AND : '&&';
 IDENTIFIER : [a-zA-Z_][a-zA-Z0-9_']*;

WS
	:	(' ' | '\r' | '\n' | '\t') -> channel(HIDDEN)
	;

NL : ';'+;
