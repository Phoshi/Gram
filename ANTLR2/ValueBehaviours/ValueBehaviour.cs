using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    interface ValueBehaviour {
        IValue BinaryOperator(IValue operand1, string op, IValue operand2);
        IValue UnaryOperator(IValue operand1, string op);
    }
}
