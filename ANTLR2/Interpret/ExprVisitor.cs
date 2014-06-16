using ANTLR2.Interpret;
using ANTLR2.Tree;
using ANTLR2.Value;
using ANTLR2.ValueBehaviour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class ExprVisitor : gramBaseVisitor<IValue> {
        private readonly Environment environment;

        public Environment Environment { get { return environment; } }

        private ExprVisitor newScope() {
            return new ExprVisitor(environment.GetChildEnvironment());
        }

        public ExprVisitor(Environment environment) {
            this.environment = environment;
        }

        public ExprVisitor() {
            this.environment = generateBuiltins();
        }

        private Environment generateBuiltins() {
            return new Environment() {
                new Binding("print", ValueFactory.make(x=>{Console.WriteLine(x);return ValueFactory.make();})),
                new Binding("typeof", ValueFactory.make(x=>{
                        var t = x.Type;
                        return ValueFactory.make(t);
                })),
                new Binding("import", ValueFactory.make((IValue x)=>{
                    var path = x.Get<string>();
                    var script = File.ReadAllText(path);
                    var interpreter = new GramInterpreter();
                    return interpreter.Execute(script);
                })),
                new Binding("local", ValueFactory.make((IValue x)=>{
                    var module = x.Get<Environment>();
                    foreach (var bind in module) {
                        this.environment.Add(bind);
                    }
                    return x;
                })),
                new Binding("Any", ValueFactory.make(Type.Of(ValueType.ANY))),
                new Binding("length", ValueFactory.make(x=>{
                    return ValueFactory.make(x.Get<IEnumerable<IValue>>().Count());
                })),
                new Binding("Int", ValueFactory.make(new Type(ValueType.INTEGER))),
                new Binding("Type", ValueFactory.make(Type.Of(ValueType.TYPE))),
                new Binding("String", ValueFactory.make(Type.Of(ValueType.STRING))),
                new Binding("DEBUG", ValueFactory.make(0)),
            };
        }

        public override IValue Visit(Antlr4.Runtime.Tree.IParseTree tree) {
            if (environment["DEBUG"].Value.Equals(ValueFactory.make(true))) {
                Console.WriteLine("Interpreting: " + tree.GetText());
                Console.WriteLine("========================================================");
                Console.WriteLine("In environment: " + environment);
                Console.WriteLine("========================================================");
                var result = base.Visit(tree);
                Console.WriteLine("Resulting in: " + result);
                Console.WriteLine("========================================================");
                return result;
            }
            return base.Visit(tree);
        }
        public override IValue VisitStatement_expr(gramParser.Statement_exprContext context) {
            return Visit(context.expr());
        }

        public override IValue VisitInt(gramParser.IntContext context) {
            return ValueFactory.make(int.Parse(context.INT().GetText()));
        }

        public override IValue VisitString(gramParser.StringContext context) {
            var str = context.STR().GetText();
            return ValueFactory.make(str.Substring(1, str.Length - 2));
        }

        public override IValue VisitUnary_operators(gramParser.Unary_operatorsContext context) {
            var result = Visit(context.expr());
            return ValueBehaviourFactory.GetBehaviour(result).UnaryOperator(result, context.SUB().GetText());
        }

        public override IValue VisitAddSub(gramParser.AddSubContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return left.Operator(context.op.Text, right);
        }

        public override IValue VisitMulDiv(gramParser.MulDivContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));


            return left.Operator(context.op.Text, right);
        }

        public override IValue VisitLogicaloperator(gramParser.LogicaloperatorContext context) {
            var left = Visit(context.expr(0));
            if (context.op.Type == gramParser.OR) {
                if (left.Equals(ValueFactory.make(true))) {
                    return ValueFactory.make(true);
                } else if (Visit(context.expr(1)).Equals(ValueFactory.make(true))) {
                    return ValueFactory.make(true);
                } else {
                    return ValueFactory.make(false);
                }
            } else if (context.op.Type == gramParser.AND) {
                if (left.Equals(ValueFactory.make(false))) {
                    return ValueFactory.make(false);
                } else if (Visit(context.expr(1)).Equals(ValueFactory.make(true))) {
                    return ValueFactory.make(true);
                } else {
                    return ValueFactory.make(false);
                }
            }

            return ValueFactory.make(false);
        }

        public override IValue VisitParens(gramParser.ParensContext context) {
            return Visit(context.expr());
        }

        public override IValue VisitVariable(gramParser.VariableContext context) {
            return environment[context.IDENTIFIER().GetText()].Value;
        }


        public override IValue VisitFunctype(gramParser.FunctypeContext context) {
            var paramType = Visit(context.expr(0));
            var returnType = Visit(context.expr(1));
            return ValueFactory.make(new FunctionType(paramType, returnType));
        }

        public override IValue VisitPredtype(gramParser.PredtypeContext context) {
            var type = Visit(context.expr(0));
            var predicate = Visit(context.expr(1));
            return ValueFactory.make(new Type(type, predicate, context.expr(1).GetText()));
        }

        public override IValue VisitStatement_assignment(gramParser.Statement_assignmentContext context) {
            var value = Visit(context.expr());

            var bindings = setBindings(context.binding(), value);
            if (bindings.Count == 1) {
                return bindings.First().Value;
            } else {
                return ValueFactory.make(bindings.Select(bind => bind.Value));
            }
        }

        public override IValue VisitStatement_assignment_readonly(gramParser.Statement_assignment_readonlyContext context) {
            var value = Visit(context.expr());

            var bindings = setBindings(context.binding(), value);
            foreach (var bind in bindings){
                bind.ReadOnly = true;
            }

            if (bindings.Count == 1) {
                return bindings.First().Value;
            } else {
                return ValueFactory.make(bindings.Select(bind => bind.Value));
            }
        }

        public override IValue VisitVariable_assignment(gramParser.Variable_assignmentContext context) {
            var varname = context.IDENTIFIER().GetText();
            environment[varname].Value = Visit(context.expr());
            return environment[varname].Value;
        }

        public override IValue VisitEquality(gramParser.EqualityContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return left.Operator(context.op.Text, right);
        }

        public override IValue VisitInequality(gramParser.InequalityContext context) {
            var result = Visit(context.expr());
            return result.Operator(context.INEQ().GetText());
        }

        public override IValue VisitStatement_func_call(gramParser.Statement_func_callContext context) {
            var func = Visit(context.expr(0));
            return func.Operator("()", Visit(context.expr(1)));
        }

        public override IValue VisitFunc_literal(gramParser.Func_literalContext context) {
            var explorer = new BindingExplorer(this);
            var bindTree = explorer.Visit(context.binding());
            var typeTree = bindTreeToTypeTree(bindTree);
            Func<IValue, IValue> func = x => {
                var correctness = new TypeChecker(typeTree).Check(x);
                if (!correctness) {
                    throw new TypeException("Type violation!");
                }
                var scope = newScope();
                scope.setBindings(context.binding(), x);
                return scope.Visit(context.expr());
            };
            return ValueFactory.make(func, typeTree);
        }

        public override IValue VisitFunc_literal_typed(gramParser.Func_literal_typedContext context){
            var explorer = new BindingExplorer(this);
            var bindTree = explorer.Visit(context.binding());
            var typeTree = bindTreeToTypeTree(bindTree);

            var resultType = Visit(context.expr(0));

            Func<IValue, IValue> func = x => {
                var correctness = new TypeChecker(typeTree).Check(x);
                if (!correctness) {
                    throw new TypeException("Type violation!");
                }
                var scope = newScope();
                scope.setBindings(context.binding(), x);
                return scope.Visit(context.expr(1));
            };
            return ValueFactory.make(func, typeTree, resultType);
        }

        public IValue bindTreeToTypeTree(Tree<Binding> bindTree) {
            if (bindTree.Values.Count > 1) {
                return ValueFactory.make(bindTree.Values.Select(bind => ValueFactory.make(bind.Type)));
            } else if (bindTree.Values.Count == 1) {
                return bindTree.Values.Select(bind => ValueFactory.make(bind.Type)).Single();
            } else if (bindTree.Children.Count > 0) {
                return ValueFactory.make(bindTree.Children.Select(bind => bindTreeToTypeTree(bind)));
            } else {
                return ValueFactory.make(new List<IValue>());
            }
        }

        public override IValue VisitBlockexpr(gramParser.BlockexprContext context) {
            return ValueFactory.make(context.expr().Select(Visit));
        }

        public override IValue VisitList_index(gramParser.List_indexContext context) {
            var list = Visit(context.expr(0));
            if (!list.Type.Check(ValueType.LIST) && !list.Type.Check(ValueType.STRING)) {
                throw new TypeException("Indexing must be done on list type!");
            }
            var index = Visit(context.expr(1));
            if (!index.Type.Check(ValueType.INTEGER)) {
                throw new TypeException("List index must be integral!");
            }

            return list.Operator("[]", index);
        }

        public override IValue VisitIf(gramParser.IfContext context) {
            var cond = Visit(context.expr(0));
            if (!cond.Type.Check(ValueType.INTEGER)) {
                throw new TypeException("If condition must be integral");
            }
            if (cond.Equals(ValueFactory.make(true))) {
                return Visit(context.expr(1));
            } else {
                if (context.expr().Count > 2) {
                    return Visit(context.expr(2));
                }
                return ValueFactory.make();
            }
        }

        public override IValue VisitFor(gramParser.ForContext context) {
            var iterable = Visit(context.expr(0));
            if (!iterable.Type.Check(ValueType.LIST)) {
                throw new TypeException("For iterable must be collection");
            }

            var results = new List<IValue>();
            foreach (var item in iterable.Get<IEnumerable<IValue>>()){
                var scope = newScope();
                scope.setBindings(context.binding(), item);
                results.Add(scope.Visit(context.expr(1)));
            }
            return ValueFactory.make(results);
        }

        public override IValue VisitWhile(gramParser.WhileContext context) {
            var conditional = Visit(context.expr(0));
            if (!conditional.Type.Check(ValueType.INTEGER)) {
                throw new TypeException("While condition must be integral.");
            }

            var results = new List<IValue>();
            while (conditional.Equals(ValueFactory.make(true))) {
                var scope = newScope();
                results.Add(scope.Visit(context.expr(1)));

                conditional = Visit(context.expr(0));
            }

            return ValueFactory.make(results);
        }

        public override IValue VisitModule_literal(gramParser.Module_literalContext context) {
            var moduleScope = newScope();
            moduleScope.Visit(context.expr());
            return ValueFactory.make(moduleScope.Environment);
        }

        public override IValue VisitModule_dereference(gramParser.Module_dereferenceContext context) {
            var module = Visit(context.expr());
            var identifier = context.IDENTIFIER().GetText();
            return module.Get<Environment>()[identifier].Value;
        }

        public override IValue VisitWhere(gramParser.WhereContext context) {
            var envScope = newScope();
            var environment = envScope.Visit(context.expr(1));
            return envScope.Visit(context.expr(0));
        }

        public override IValue VisitPattern_match(gramParser.Pattern_matchContext context) {
            var pattern = Visit(context.expr(0));
            var match = Visit(context.expr(1));
            foreach (var func in match.Get<IEnumerable<IValue>>()) {
                if (func is FunctionValue) {
                    var f = func as FunctionValue;
                    var t = f.Type as FunctionType;
                    var typeChecker = new TypeChecker(t.Parameter);
                    if (typeChecker.Check(pattern)) {
                        return f.Operator("()", pattern);
                    }
                }
            }
            throw new GramException("Non-exhaustive match in pattern!");
        }

        public IList<Binding> setBindings(gramParser.BindingContext context, IValue val){
            var bindingExplorer = new BindingAssigner(this, val);
            var bindings = bindingExplorer.Visit(context);
            var typeTree = bindTreeToTypeTree(bindings);
            var typeCheck = new TypeChecker(typeTree).Check(val);
            var binds = bindings.GetAllValues().ToList();

            foreach (var bind in binds) {
                environment.Add(bind);
            }

            return binds;
        }
    }
}
