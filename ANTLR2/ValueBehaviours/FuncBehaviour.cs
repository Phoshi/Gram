using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class FuncBehaviour : ValueBehaviour {
        public IValue BinaryOperator(IValue operand1, string op, IValue operand2) {
            switch (op) {
                case "()": return operand1.Get<Func<IValue, IValue>>()(operand2);
            }

            throw new InvalidOperationException();
        }

        public IValue UnaryOperator(IValue operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
