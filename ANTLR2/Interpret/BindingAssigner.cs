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
            if (name == "_") {
                return new Tree<Binding>();
            }
            if (context.expr() != null) {
                var type = interpreter.Visit(context.expr());
                if (type.Type.Check(ValueType.TYPE)) {
                    return new Tree<Binding>(new Binding(name, type.Get<IType>(), value));
                } else if (type.Type.Check(ValueType.LIST)) {
                    return new Tree<Binding>(new Binding(name, new ListType(type.Get<IEnumerable<IValue>>(), name), value));
                }
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

        public override Tree<Binding> VisitBinding_trailing(gramParser.Binding_trailingContext context) {
            var numBindings = context.binding().Count;
            var bindings = context.binding().Take(numBindings - 1).Zip(value.Get<IEnumerable<IValue>>(), (binding, subValue) => new BindingAssigner(interpreter, subValue).Visit(binding));

            var tree = new Tree<Binding>();
            foreach (var binding in bindings) {
                tree.Add(binding);
            }

            var trailing = new BindingAssigner(interpreter, 
                ValueFactory.make(value.Get<IEnumerable<IValue>>().Skip(numBindings - 1)));
            tree.Add(trailing.Visit(context.binding().Last()));

            return tree;
        }
    }
}
