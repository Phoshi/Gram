using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class NullBehaviour : ValueBehaviour {
        public Value BinaryOperator(Value operand1, string op, Value operand2) {
            switch (op) {
                case "==": return ValueFactory.make(0);
                default:
                    return operand1;
            }
        }


        public Value UnaryOperator(Value operand1, string op) {
            return operand1;
        }
    }
}
