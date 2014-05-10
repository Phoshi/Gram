using ANTLR2.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Interpret {
    class BindingExplorer : gramBaseVisitor<Tree<Binding>>{
        private readonly ExprVisitor interpreter;
        private readonly Value value;
        public BindingExplorer(ExprVisitor interpreter, Value value) {
            this.interpreter = interpreter;
            this.value = value;
        }

        public override Tree<Binding> VisitVariable(gramParser.VariableContext context) {
            var name = context.IDENTIFIER().GetText();
            if (context.type() != null) {
                var type = interpreter.Visit(context.type());
                return new Tree<Binding>(new Binding(name, type.AsType, value));
            }

            return new Tree<Binding>(new Binding(name, value));
        }

        public override Tree<Binding> VisitBinding_multiple(gramParser.Binding_multipleContext context) {
            var bindings = context.binding().Zip(value.AsList, (binding, subValue) => new BindingExplorer(interpreter, subValue).Visit(binding));
            var tree = new Tree<Binding>();
            foreach (var binding in bindings) {
                tree.Add(binding);
            }

            return tree;
        }
    }
}
