using ANTLR2.Tree;
using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Interpret {
    class BindingAssigner : gramBaseVisitor<Tree<Binding>>{
        private readonly ExprVisitor interpreter;
        private readonly IValue value;
        public BindingAssigner(ExprVisitor interpreter, IValue value) {
            this.interpreter = interpreter;
            this.value = value;
        }

        public override Tree<Binding> VisitVariable(gramParser.VariableContext context) {
            var name = context.IDENTIFIER().GetText();
            if (context.type() != null) {
                var type = interpreter.Visit(context.type());
                return new Tree<Binding>(new Binding(name, type.Get<Type>(), value));
            }

            return new Tree<Binding>(new Binding(name, value));
        }

        public override Tree<Binding> VisitBinding_multiple(gramParser.Binding_multipleContext context) {
            var bindings = context.binding().Zip(value.Get<IEnumerable<IValue>>(), (binding, subValue) => new BindingAssigner(interpreter, subValue).Visit(binding));
            var tree = new Tree<Binding>();
            foreach (var binding in bindings) {
                tree.Add(binding);
            }

            return tree;
        }
    }
}
