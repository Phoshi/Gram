using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class FuncBehaviour : ValueBehaviour {
        public Value BinaryOperator(Value operand1, string op, Value operand2) {
            switch (op) {
                case "()": return operand1.AsFunc(operand2);
            }

            throw new InvalidOperationException();
        }

        public Value UnaryOperator(Value operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
