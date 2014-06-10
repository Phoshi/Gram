using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class StringToIntBehaviour : ValueBehaviour{
        public Value.IValue BinaryOperator(Value.IValue operand1, string op, Value.IValue operand2) {
            if (op == "[]") {
                var str = operand1.Get<string>();
                var index = operand2.Get<int>();
                if (index < 0) index = str.Length + index;
                return ValueFactory.make(str[index].ToString());
            }

            throw new InvalidOperationException();
        }

        public Value.IValue UnaryOperator(Value.IValue operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
