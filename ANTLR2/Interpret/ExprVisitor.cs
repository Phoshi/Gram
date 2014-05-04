using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class ExprVisitor : gramBaseVisitor<Value> {
        private Environment environment = new Environment() {
            new Binding("print", ValueFactory.make(x=>{Console.WriteLine(x);return ValueFactory.make();})),
            new Binding("typeof", ValueFactory.make(x=>{
                    var t = x.Type;
                    return ValueFactory.make(t);
            })),
            new Binding("length", ValueFactory.make(x=>{
                return ValueFactory.make(x.AsList.Count());
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

        public override Value Visit(Antlr4.Runtime.Tree.IParseTree tree) {
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
        public override Value VisitStatement_expr(gramParser.Statement_exprContext context) {
            return Visit(context.expr());
        }

        public override Value VisitInt(gramParser.IntContext context) {
            return ValueFactory.make(int.Parse(context.INT().GetText()));
        }

        public override Value VisitAddSub(gramParser.AddSubContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return left.Operator(context.op.Text, right);
        }

        public override Value VisitMulDiv(gramParser.MulDivContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));


            return left.Operator(context.op.Text, right);
        }

        public override Value VisitLogicaloperator(gramParser.LogicaloperatorContext context) {
            var left = Visit(context.expr(0));
            if (context.op.Type == gramParser.OR) {
                if (left.AsInt != 0) {
                    return environment["true"].Value;
                } else if (Visit(context.expr(1)).AsInt != 0) {
                        return environment["true"].Value;
                } else {
                    return environment["false"].Value;
                }
            } else if (context.op.Type == gramParser.AND) {
                if (left.AsInt == 0) {
                    return environment["false"].Value;
                } else if (Visit(context.expr(1)).AsInt != 0) {
                    return environment["true"].Value;
                } else {
                    return environment["false"].Value;
                }
            }

            return environment["false"].Value;
        }

        public override Value VisitParens(gramParser.ParensContext context) {
            return Visit(context.expr());
        }

        public override Value VisitVariable(gramParser.VariableContext context) {
            return environment[context.IDENTIFIER().GetText()].Value;
        }

        public override Value VisitRawtype(gramParser.RawtypeContext context) {
            return environment[context.IDENTIFIER().GetText()].Value;
        }

        public override Value VisitFunctype(gramParser.FunctypeContext context) {
            return ValueFactory.make(Type.Of(ValueType.FUNCTION));
        }

        public override Value VisitPredtype(gramParser.PredtypeContext context) {
            var type = Visit(context.type());
            var predicate = Visit(context.expr());
            return ValueFactory.make(new Type(type.AsType.RawTypeOf, predicate, context.expr().GetText()));
        }

        public override Value VisitListtype(gramParser.ListtypeContext context) {
            return ValueFactory.make(Type.Of(ValueType.LIST));
        }

        public override Value VisitStatement_assignment(gramParser.Statement_assignmentContext context) {
            var value = Visit(context.expr());

            var bindings = setBindings(context.binding(), value);
            if (bindings.Count == 1) {
                return bindings.First().Value;
            } else {
                return ValueFactory.make(bindings.Select(bind => bind.Value));
            }
        }

        public override Value VisitStatement_assignment_readonly(gramParser.Statement_assignment_readonlyContext context) {
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

        public override Value VisitVariable_assignment(gramParser.Variable_assignmentContext context) {
            var varname = context.IDENTIFIER().GetText();
            environment[context.IDENTIFIER().GetText()].Value = Visit(context.expr());
            return environment[varname].Value;
        }

        public override Value VisitEquality(gramParser.EqualityContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            return left.Operator(context.op.Text, right);
        }

        public override Value VisitInequality(gramParser.InequalityContext context) {
            var result = Visit(context.expr());
            return result.Operator(context.INEQ().GetText());
        }

        public override Value VisitStatement_func_call(gramParser.Statement_func_callContext context) {
            var func = Visit(context.expr(0));
            return func.Operator("()", Visit(context.expr(1)));
        }

        public override Value VisitFunc_literal(gramParser.Func_literalContext context) {
            Func<Value, Value> func = x => {
                var scope = newScope();
                scope.setBindings(context.binding(), x);
                return scope.Visit(context.expr());
            };
            return ValueFactory.make(func);
        }

        public override Value VisitBlockexpr(gramParser.BlockexprContext context) {
            return ValueFactory.make(context.expr().Select(Visit));
        }

        public override Value VisitList_index(gramParser.List_indexContext context) {
            var list = Visit(context.expr(0));
            var index = Visit(context.expr(1));

            return list.Operator("[]", index);
        }

        public override Value VisitIf(gramParser.IfContext context) {
            var cond = Visit(context.expr(0)).AsInt;
            if (cond != 0) {
                return Visit(context.expr(1));
            } else {
                if (context.expr().Count > 2) {
                    return Visit(context.expr(2));
                }
                return ValueFactory.make();
            }
        }

        public override Value VisitFor(gramParser.ForContext context) {
            var iterable = Visit(context.expr(0));

            var results = new List<Value>();
            foreach (var item in iterable.AsList){
                var scope = newScope();
                scope.setBindings(context.binding(), item);
                results.Add(scope.Visit(context.expr(1)));
            }
            return ValueFactory.make(results);
        }

        public override Value VisitWhile(gramParser.WhileContext context) {
            var conditional = Visit(context.expr(0));

            var results = new List<Value>();
            while (conditional.AsInt == 1) {
                var scope = newScope();
                results.Add(scope.Visit(context.expr(1)));

                conditional = Visit(context.expr(0));
            }

            return ValueFactory.make(results);
        }

        public IList<Binding> setBindings(gramParser.BindingContext context, Value val){
            if (val.Type.RawTypeOf == ValueType.LIST && context.variable().Count > 1) {
                return _setBindings(context, val.AsList.ToList());
            } else {
                return new[]{_setBindings(context, val)};
            }
        }

        private Binding _setBindings(gramParser.BindingContext context, Value val){
            var name =  context.variable(0).IDENTIFIER().GetText();
            var typeid = context.variable(0).type();

            Type type;
            if (typeid != null) {
                type = Visit(typeid).AsType;
            } else {
                type = val.Type;
            }

            var binding = new Binding(name, type, val);
            environment.Add(binding);
            return binding;
        }

        private IList<Binding> _setBindings(gramParser.BindingContext context, IList<Value> vals){
            var bindings = new List<Binding>();
            int iteration = 0;
            foreach (var val in vals){
                var name = context.variable(iteration).IDENTIFIER().GetText();
                var typeid = context.variable(iteration).type();

                Type type;
                if (typeid != null) {
                    type = Visit(typeid).AsType;
                } else {
                    type = vals[iteration].Type;
                }

                var binding = new Binding(name, type, vals[iteration]);
                environment.Add(binding);
                bindings.Add(binding);

                iteration++;
            }
            return bindings;
        }
    }
}
