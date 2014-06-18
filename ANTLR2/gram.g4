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
			| expr op=(MUL|DIV|MOD) expr# MulDiv
			| expr op=(ADD|SUB) expr	# AddSub
			| expr op=(EQ|LT|GT) expr	# equality
			| expr op=(OR|AND) expr		# logicaloperator
			| SUB expr					# unary_operators
			| INEQ expr					# inequality
			| func						# expr_func
			| variable					# expr_variable
			| expr 'match' expr			# pattern_match
			| expr where_expr			# where
			| expr '('? expr ')'? 		# statement_func_call
			| expr '()'					# statement_empty_func_call
			| expr '[' expr ']'			# list_index
			| '(' expr ')'				# parens
			| expr '::' IDENTIFIER		# module_dereference
			| 'module' expr				# module_literal
			| expr '->' expr			# functype
			| expr '<' expr '>'			# predtype
			| 'var' binding '=' expr	# statement_assignment
			| 'val' binding '=' expr	# statement_assignment_readonly
			| IDENTIFIER '=' expr		# variable_assignment
			| INT						# int
			| STR						# string
			;

where_expr	: 'where' '{' (expr NL)* expr? '}'
			| 'where' expr
			;

variable	: IDENTIFIER
			| IDENTIFIER ':' expr
			;

binding		: variable								#binding_single
			| '{' (binding NL)* binding? '}'		#binding_multiple
			| '{' (binding NL)+ binding '...' '}'	#binding_trailing
			;

if			: 'if' '(' expr ')' expr ('else' expr)?
			;

for			: 'for' '(' binding ':' expr ')' expr
			;

while		: 'while' '(' expr ')' expr
			;

func		: binding '->' expr '=>' expr		# func_literal_typed
			| binding '=>' expr					# func_literal
			;

/*
 * Lexer Rules
 */

 INT : [0-9]+;
 MUL : '*';
 DIV : '/';
 ADD : '+';
 SUB : '-';
 MOD : '%';
 EQ  : '==';
 GT  : '>';
 LT  : '<';
 INEQ: '!';
 OR  : '||';
 AND : '&&';
 IDENTIFIER : [a-zA-Z_][a-zA-Z0-9_']*;

 STR : '"' ANYTHING '"';
 ANYTHING : .*?;

WS
	:	('#{' .*? '}#' | ' ' | '\r' | '\n' | '\t') -> channel(HIDDEN)
	;

NL : ';'+;
