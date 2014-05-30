using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class ListToOtherBehaviour : ValueBehaviour {
        public virtual IValue BinaryOperator(IValue operand1, string op, IValue operand2) {
            switch (op) {
                case "+": {
                        var list = new List<IValue>(operand1.Get<IEnumerable<IValue>>());
                        list.Add(operand2);
                        return ValueFactory.make(list);
                    }
                case "==": {
                    return ValueFactory.make(false);
                    }
            }
            throw new InvalidOperationException();
        }


        public IValue UnaryOperator(IValue operand1, string op) {
            return ValueBehaviourFactory.GetBehaviour(operand1).UnaryOperator(operand1, op);
        }
    }
}
