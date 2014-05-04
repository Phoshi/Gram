using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class ListToIntBehaviour : ListToOtherBehaviour {
        public override Value BinaryOperator(Value operand1, string op, Value operand2){
            if (op == "[]") {
                var index = operand2.AsInt;
                var list = operand1.AsList;
                if (index >= 0) {
                    return list.Skip(index).First();
                } else {
                    return list.Reverse().Skip(-index - 1).First();
                }
            }
            return base.BinaryOperator(operand1, op, operand2);
        }

    }
}
