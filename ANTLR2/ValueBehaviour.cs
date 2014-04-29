using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    interface ValueBehaviour {
        Value BinaryOperator(Value operand1, string op, Value operand2);
    }
}
