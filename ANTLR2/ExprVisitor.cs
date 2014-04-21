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
            new Binding("int", ValueFactory.make(new Type(ValueType.INTEGER))),
            new Binding("true", ValueFactory.make(1)),
            new Binding("false", ValueFactory.make(0)),
        };

        private ExprVisitor newScope() {
            return new ExprVisitor(environment.GetChildEnvironment());
        }

        public ExprVisitor(Environment environment) {
            this.environment = environment;
        }

        public ExprVisitor() {}

        public override Value VisitStatement_expr(gramParser.Statement_exprContext context) {
            return Visit(context.expr());
        }

        public override Value VisitInt(gramParser.IntContext context) {
            return ValueFactory.make(int.Parse(context.INT().GetText()));
        }

        public override Value VisitAddSub(gramParser.AddSubContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            if (context.op.Type == gramParser.ADD) {
                return left + right;
            }
            return left - right;
        }

        public override Value VisitMulDiv(gramParser.MulDivContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));


            if (context.op.Type == gramParser.MUL) {
                return left * right;
            }
            return left / right;
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
            var varname = context.variable().IDENTIFIER().GetText();
            var vartype = context.variable().type();
            var value = Visit(context.expr());

            Type type;
            if (vartype != null) {
                type = Visit(vartype).AsType;
            } else {
                type = value.Type;
            }

            environment.Add(new Binding(varname, type, value));
            return environment[varname].Value;
        }

        public override Value VisitStatement_assignment_readonly(gramParser.Statement_assignment_readonlyContext context) {
            var varname = context.variable().IDENTIFIER().GetText();
            var vartype = context.variable().type();
            var value = Visit(context.expr());

            Type type;
            if (vartype != null) {
                type = Visit(vartype).AsType;
            } else {
                type = value.Type;
            }

            environment.Add(new Binding(varname, type, value) { ReadOnly = true });
            return environment[varname].Value;
        }

        public override Value VisitVariable_assignment(gramParser.Variable_assignmentContext context) {
            var varname = context.IDENTIFIER().GetText();
            environment[context.IDENTIFIER().GetText()].Value = Visit(context.expr());
            return environment[varname].Value;
        }

        public override Value VisitEquality(gramParser.EqualityContext context) {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            if (context.op.Type == gramParser.EQ) {
                if (left.Equals(right)) {
                    return ValueFactory.make(1);
                } else {
                    return ValueFactory.make(0);
                }
            }
            if (context.op.Type == gramParser.LT) {
                if (left.AsInt < right.AsInt) {
                    return environment["true"].Value;
                } else {
                    return environment["false"].Value;
                }
            }
            if (context.op.Type == gramParser.GT) {
                if (left.AsInt > right.AsInt) {
                    return environment["true"].Value;
                } else {
                    return environment["false"].Value;
                }
            }
            throw new NotImplementedException();
        }

        public override Value VisitInequality(gramParser.InequalityContext context) {
            var result = Visit(context.expr());
            if (result.AsInt == 0) {
                return ValueFactory.make(1);
            } else {
                return ValueFactory.make(0);
            }
        }

        public override Value VisitStatement_func_call(gramParser.Statement_func_callContext context) {
            var func = Visit(context.expr(0)).AsFunc;
            return func(Visit(context.expr(1)));
        }

        public override Value VisitFunc_literal(gramParser.Func_literalContext context) {
            Func<Value, Value> func = x => {
                var identifier = context.IDENTIFIER().GetText();
                var scope = newScope();
                scope.environment[identifier] = new Binding(identifier, x);
                return scope.Visit(context.expr());
            };
            return ValueFactory.make(func);
        }

        public override Value VisitBlockexpr(gramParser.BlockexprContext context) {
            return ValueFactory.make(context.expr().Select(Visit));
        }

        public override Value VisitList_index(gramParser.List_indexContext context) {
            var list = Visit(context.expr(0)).AsList;
            var index = Visit(context.expr(1)).AsInt;

            return list.Skip(index).First();
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
            var identifier = context.IDENTIFIER().GetText();
            var iterable = Visit(context.expr(0));

            var results = new List<Value>();
            foreach (var item in iterable.AsList){
                var scope = newScope();
                scope.environment[identifier] = new Binding(identifier, item);
                results.Add(scope.Visit(context.expr(1)));
            }
            return ValueFactory.make(results);
        }
    }
}
