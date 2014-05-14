using ANTLR2.ValueBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class FunctionValue : IValue {
        private readonly Func<IValue, IValue> innerFunction;
        public FunctionValue(Func<IValue, IValue> func) {
            this.innerFunction = func;
        }
        public IValue Operator(string op) {
            return ValueBehaviourFactory.GetBehaviour(this).UnaryOperator(this, op);
        }

        public IValue Operator(string op, IValue operand) {
            return ValueBehaviourFactory.GetBehaviour(this, operand).BinaryOperator(this, op, operand);
        }

        public Type Type {
            get { return Type.Of(ValueType.FUNCTION); }
        }

        public T Get<T>() {
            return (T)(object)innerFunction;
        }

        public override string ToString() {
            return "Any=>Any";
        }
    }
}
