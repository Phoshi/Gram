Gram
====

What?
-----
Gram is a small language experiment. It is not designed to be useful, or polished, or in any way production-ready, and it probably never will be.

In Gram, everything is an expression, meaning that every command, function, and keyword has some result, even if that result is Unit. Every value is first class, including types, and functions. The primary justification for Gram's existence is the system of dependant predicated typing. It is also an experiment in functions which are always a mapping from one input set to one output set, sidestepping the question of arity.

Everything is an Expression.
----

	$ 5;
	> 5;
	$ 2 + 2;
	> 4;
	$ if (true) 3 else 4;
	> 3
	$ for (x : range{0; 10}) x;
	> {0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10}

There's no concept of a statement in Gram. If it executes, it has a return type.

Everything is First Class
----

	$ int;
	> Type: INTEGER
	$ val number = int;
	> Type: INTEGER
	$ x=>x;
	> Type: FUNCTION
	$ val f = x=>x;
	> Type: FUNCTION

If it exists, you can bind it to a variable. You can bind that variable to a variable. You can use that variable anywhere you would have used the literal.

Dependant Predicated Typing
----

	$ var x: int = 3;
	> 3
	$ var y: int<x=>x>0> = 3;
	> 3;
	$ x = -1;
	> -1;
	$ y = x;
	> Type violation!

Types can have more than just a base type, they can be constrained arbitrarily. The variable y in the above example is of a type which represents only positive integers. This is predicated typing, which allows you to freely mix in deeper logic into the type system, such that an input or output outside of the defined scope of the function can be caught.

	$ val a: int = 9;
	$ var z: int<z=>z>a> = 10;
	> 10;

Types can also be dependant on values, further encoding logic into the function signature. For the time being, types can only be dependant on *val*ues, not *var*iables.

Functions
---------

Functions can only have one input value. This value can be a collection, which can use destructuring assignment to simulate multiple values. A function can only return one value, but this can again be a collection, which can be destructured at the call site.

	$ val f = {a; b: int; c: int<c=>c>b>} => {a; a + b; a + b + c};

	$ f{1;2;3}
	> {1; 3; 6}
	$ val {a; b; c} = f{1;2;3};
	> {1; 3; 6}

Here, `f` is a function which accepts one argument, which should be a collection of three items. If those items don't conform to the expected types, it is a type violation, and your application will not work. Note the dependant value `c`, which is required to be larger than `b`. The return set is then destructured and bound to three bindings in the current scope.

A Crash Course in Syntax
==========================

Expression
----------

Everything is an expression. If you type it and it compiles, it is an expression.

Assignment
-------
	val variableName: Type = expression;

Creates a new value binding, which cannot be changed, of the given type. It is bound to the result of the expression. The type can be ommitted, in which case it will be inferred.

	var variableName: Type = expression;

Creates a new variable binding, which acts like a value binding except it can be changed. The type constraints will be checked, and you cannot assign a value which the type does not fit.


If
----
	if (predicate) expression-if-true else expression-if-false

An if statement returns one expression if the predicate returns true, otherwise it returns the other expression.

Block/Collections
-----------------

	{expression; expression; expression}

A collection block returns a list of every expression within it.

Collection Indexing
--------------------

	collection[integer]

Returns the given index from the collection. If given a negative index, indexes from the end instead.

Function Call
-------------

	function(input)

Calls the given function. The brackets can be ommitted.

Note
====

Predicated types are impossible to prove at compile time in the general case. This language is an experiment to see how close I can get.