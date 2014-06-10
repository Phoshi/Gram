using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class StringBehaviour : ValueBehaviour {
        public Value.IValue BinaryOperator(Value.IValue operand1, string op, Value.IValue operand2) {
            switch (op) {
                case "+": {
                    var str1 = operand1.Get<string>();
                    var str2 = operand2.Get<string>();
                    return ValueFactory.make(str1 + str2);
                    }
                case "==": {
                    var str1 = operand1.Get<string>();
                    var str2 = operand2.Get<string>();
                    return ValueFactory.make(str1.Equals(str2));
                    }
            }
            throw new InvalidOperationException();
        }

        public Value.IValue UnaryOperator(Value.IValue operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
