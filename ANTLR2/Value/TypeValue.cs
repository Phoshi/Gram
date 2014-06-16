using ANTLR2.ValueBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class TypeValue : IValue {
        private readonly IType type;
        private readonly String name;

        public TypeValue(IType type) {
            this.type = type;
            this.name = type.ToString();
        }

        public TypeValue(IType type, string name) {
            this.type = type;
            this.name = name;
            //type.FriendlyName = name;
        }

        public IValue Operator(string op) {
            return ValueBehaviourFactory.GetBehaviour(this).UnaryOperator(this, op);
        }

        public IValue Operator(string op, IValue operand) {
            return ValueBehaviourFactory.GetBehaviour(this, operand).BinaryOperator(this, op, operand);
        }

        public IType Type {
            get { return new Type(ValueType.TYPE); }
        }

        public IValue Constrain(Binding binding) {
            return new TypeValue(type, binding.Name);
        }

        public T Get<T>() {
            return (T)(object)type;
        }

        public override string ToString() {
            return name;
        }
    }
}
