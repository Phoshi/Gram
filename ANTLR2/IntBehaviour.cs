using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class IntBehaviour : ValueBehaviour {
        public Value BinaryOperator(Value operand1, string op, Value operand2) {
            switch (op){
                case "+":   return ValueFactory.make(operand1.AsInt + operand2.AsInt);
                case "-":   return ValueFactory.make(operand1.AsInt + operand2.AsInt);
                case "/":   return ValueFactory.make(operand1.AsInt / operand2.AsInt);
                case "*":   return ValueFactory.make(operand1.AsInt * operand2.AsInt);
                case "==":  return ValueFactory.make(operand1.AsInt == operand2.AsInt ? 1 : 0);
                case ">":   return ValueFactory.make(operand1.AsInt > operand2.AsInt ? 1 : 0);
                case "<":   return ValueFactory.make(operand1.AsInt > operand2.AsInt ? 1 : 0);
            }

            throw new InvalidOperationException();
        }
    }
}
