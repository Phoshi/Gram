using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class TypeBehaviour : ValueBehaviour {
        public Value.IValue BinaryOperator(Value.IValue operand1, string op, Value.IValue operand2) {
            if (op == "==") {
                return ValueFactory.make(operand1.Get<IType>().Check(operand2.Get<IType>()));
            }
            throw new NotImplementedException();
        }

        public Value.IValue UnaryOperator(Value.IValue operand1, string op) {
            throw new NotImplementedException();
        }
    }
}
