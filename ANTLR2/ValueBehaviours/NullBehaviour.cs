using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class NullBehaviour : ValueBehaviour {
        public IValue BinaryOperator(IValue operand1, string op, IValue operand2) {
            switch (op) {
                case "==": return ValueFactory.make(0);
                default:
                    return operand1;
            }
        }


        public IValue UnaryOperator(IValue operand1, string op) {
            return operand1;
        }
    }
}
