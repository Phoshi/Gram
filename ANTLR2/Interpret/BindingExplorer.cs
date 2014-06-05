using ANTLR2.Tree;
using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Interpret {
    class BindingExplorer : gramBaseVisitor<Tree<Binding>>{
        private readonly ExprVisitor interpreter;
        public BindingExplorer(ExprVisitor interpreter) {
            this.interpreter = interpreter;
        }

        public override Tree<Binding> VisitVariable(gramParser.VariableContext context) {
            var name = context.IDENTIFIER().GetText();
            if (context.expr() != null) {
                var type = interpreter.Visit(context.expr());
                if (type.Type.Check(ValueType.TYPE)) {
                    return new Tree<Binding>(new Binding(name, type.Get<IType>()));
                } else if (type.Type.Check(ValueType.LIST)) {
                    return new Tree<Binding>(new Binding(name, new ListType(type.Get<IEnumerable<IValue>>(), context.expr().GetText())));
                }
            }

            return new Tree<Binding>(new Binding(name));
        }

        public override Tree<Binding> VisitBinding_multiple(gramParser.Binding_multipleContext context) {
            var bindings = context.binding().Select(binding => new BindingExplorer(interpreter).Visit(binding));
            var tree = new Tree<Binding>();
            foreach (var binding in bindings) {
                tree.Add(binding);
            }

            return tree;
        }
    }
}
