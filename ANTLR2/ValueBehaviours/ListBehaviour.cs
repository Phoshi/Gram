using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class ListBehaviour : ValueBehaviour {
        public Value BinaryOperator(Value operand1, string op, Value operand2) {
            switch (op) {
                case "+":
                    var list = new List<Value>(operand1.AsList);
                    list.Add(operand2);
                    return ValueFactory.make(list);
                case "==":
                    if (operand1.AsList.Count() != operand2.AsList.Count()) {
                        return ValueFactory.make(0);
                    }
                    var areSame = operand1.AsList.Intersect(operand2.AsList).Count() == operand1.AsList.Count();
                    return ValueFactory.make(areSame ? 1 : 0);
            }

            throw new InvalidOperationException();
        }


        public Value UnaryOperator(Value operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
