using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class ListBehaviour : ValueBehaviour {
        public IValue BinaryOperator(IValue operand1, string op, IValue operand2) {
            switch (op) {
                case "+":
                    var list = new List<IValue>(operand1.Get<IEnumerable<IValue>>());
                    list.Add(operand2);
                    return ValueFactory.make(list);
                case "==":
                    if (operand1.Get<IEnumerable<IValue>>().Count() != operand2.Get<IEnumerable<IValue>>().Count()) {
                        return ValueFactory.make(0);
                    }
                    var areSame = operand1.Get<IEnumerable<IValue>>().Intersect(operand2.Get<IEnumerable<IValue>>()).Count() 
                        == operand1.Get<IEnumerable<IValue>>().Count();
                    return ValueFactory.make(areSame ? 1 : 0);
            }

            throw new InvalidOperationException();
        }


        public IValue UnaryOperator(IValue operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
