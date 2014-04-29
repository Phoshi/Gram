using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2 {
    class ListBehaviour : ValueBehaviour {
        public Value BinaryOperator(Value operand1, string op, Value operand2) {
            switch (op) {
                case "==":
                    if (operand1.AsList.Count() != operand2.AsList.Count()) {
                        return ValueFactory.make(0);
                    }
                    var areSame = operand1.AsList.Intersect(operand2.AsList).Count() == operand1.AsList.Count();
                    return ValueFactory.make(areSame ? 1 : 0);
            }

            throw new InvalidOperationException();
        }
    }
}
