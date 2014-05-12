using ANTLR2.Interpret;
using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class ExprVisitor : gramBaseVisitor<IValue> {
        private Environment environment = new Environment() {
            new Binding("print", ValueFactory.make(x=>{Console.WriteLine(x);return ValueFactory.make();})),
            new Binding("typeof", ValueFactory.make(x=>{
                    var t = x.Type;
                    return ValueFactory.make(t);
            })),
            new Binding("length", ValueFactory.make(x=>{
                return ValueFactory.make(x.Get<IEnumerable<IValue>>().Count());
            })),
            new Binding("int", ValueFactory.make(new Type(ValueType.INTEGER))),
            new Binding("true", ValueFactory.make(1)),
            new Binding("false", ValueFactory.make(0)),
            new Binding("DEBUG", ValueFactory.make(0)),
        };

        private ExprVisitor newScope() {
            return new ExprVisitor(environment.GetChildEnvironment());
        }

        public ExprVisitor(Environment environment) {
            this.environment = environment;
        }

        public ExprVisitor() {}

        public override IValue Visit(Antlr4.Runtime.Tree.IParseTree tree) {
            if (environment["DEBUG"].Value == environment["true"].Value) {
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
                    return environment["true"].Value;
                } else if (Visit(context.expr(1)).Equals(ValueFactory.make(true))) {
                        return environment["true"].Value;
                } else {
                    return environment["false"].Value;
                }
            } else if (context.op.Type == gramParser.AND) {
                if (left.Equals(ValueFactory.make(false))) {
                    return environment["false"].Value;
                } else if (Visit(context.expr(1)).Equals(ValueFactory.make(true))) {
                    return environment["true"].Value;
                } else {
                    return environment["false"].Value;
                }
            }

            return environment["false"].Value;
        }

        public override IValue VisitParens(gramParser.ParensContext context) {
            return Visit(context.expr());
        }

        public override IValue VisitVariable(gramParser.VariableContext context) {
            return environment[context.IDENTIFIER().GetText()].Value;
        }

        public override IValue VisitRawtype(gramParser.RawtypeContext context) {
            return environment[context.IDENTIFIER().GetText()].Value;
        }

        public override IValue VisitFunctype(gramParser.FunctypeContext context) {
            return ValueFactory.make(Type.Of(ValueType.FUNCTION));
        }

        public override IValue VisitPredtype(gramParser.PredtypeContext context) {
            var type = Visit(context.type());
            var predicate = Visit(context.expr());
            return ValueFactory.make(new Type(type.Get<Type>().RawTypeOf, predicate, context.expr().GetText()));
        }

        public override IValue VisitListtype(gramParser.ListtypeContext context) {
            var types = context.type().Select(t => Visit(t)).ToList();
            Func<IValue, IValue> typeChecker = l => {
                var list = l.Get<IEnumerable<IValue>>().ToList();
                var index = 0;
                foreach (var type in types){
                    if (list.Count <= index || !type.Get<Type>().Check(list[index])) {
                        return ValueFactory.make(false);
                    }
                    index = index + 1;
                }
                return ValueFactory.make(list.Count == index);
            };
            return ValueFactory.make(new Type(ValueType.LIST, ValueFactory.make(typeChecker), context.GetText()));
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
            environment[context.IDENTIFIER().GetText()].Value = Visit(context.expr());
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
            Func<IValue, IValue> func = x => {
                var scope = newScope();
                scope.setBindings(context.binding(), x);
                return scope.Visit(context.expr());
            };
            return ValueFactory.make(func);
        }

        public override IValue VisitBlockexpr(gramParser.BlockexprContext context) {
            return ValueFactory.make(context.expr().Select(Visit));
        }

        public override IValue VisitList_index(gramParser.List_indexContext context) {
            var list = Visit(context.expr(0));
            var index = Visit(context.expr(1));

            return list.Operator("[]", index);
        }

        public override IValue VisitIf(gramParser.IfContext context) {
            var cond = Visit(context.expr(0));
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

            var results = new List<IValue>();
            while (conditional.Equals(ValueFactory.make(true))) {
                var scope = newScope();
                results.Add(scope.Visit(context.expr(1)));

                conditional = Visit(context.expr(0));
            }

            return ValueFactory.make(results);
        }

        public IList<Binding> setBindings(gramParser.BindingContext context, IValue val){
            var bindingExplorer = new BindingExplorer(this, val);
            var bindings = bindingExplorer.Visit(context);
            var binds = bindings.GetAllValues().ToList();

            foreach (var bind in binds) {
                environment.Add(bind);
            }

            return binds;
        }
    }
}
