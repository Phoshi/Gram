using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class IntBehaviour : ValueBehaviour {
        public IValue BinaryOperator(IValue operand1, string op, IValue operand2) {
            switch (op){
                case "+":   return ValueFactory.make(operand1.Get<int>() + operand2.Get<int>());
                case "-":   return ValueFactory.make(operand1.Get<int>() - operand2.Get<int>());
                case "/":   return ValueFactory.make(operand1.Get<int>() / operand2.Get<int>());
                case "*":   return ValueFactory.make(operand1.Get<int>() * operand2.Get<int>());
                case "%":   return ValueFactory.make(operand1.Get<int>() % operand2.Get<int>());
                case "==":  return ValueFactory.make(operand1.Get<int>() == operand2.Get<int>());
                case ">":   return ValueFactory.make(operand1.Get<int>() > operand2.Get<int>());
                case "<":   return ValueFactory.make(operand1.Get<int>() < operand2.Get<int>());
            }

            throw new InvalidOperationException();
        }



        public IValue UnaryOperator(IValue operand1, string op) {
            switch (op) {
                case "!":
                    return ValueFactory.make(operand1 != ValueFactory.make(true));
                case "-":
                    return ValueFactory.make(-operand1.Get<int>());
            }

            throw new InvalidOperationException();
        }
    }
}
