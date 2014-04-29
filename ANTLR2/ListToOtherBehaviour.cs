using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2 {
    class ListToOtherBehaviour : ValueBehaviour {
        public Value BinaryOperator(Value operand1, string op, Value operand2) {
            switch (op) {
                case "+": 
                    var list = new List<Value>(operand1.AsList);
                    list.Add(operand2);
                    return ValueFactory.make(list);
                case "==": return ValueFactory.make(0);
            }
            throw new InvalidOperationException();
        }
    }
}
