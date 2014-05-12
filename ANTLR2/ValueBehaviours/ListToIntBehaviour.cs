using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.ValueBehaviour {
    class ListToIntBehaviour : ListToOtherBehaviour {
        public override IValue BinaryOperator(IValue operand1, string op, IValue operand2){
            if (op == "[]") {
                var index = operand2.Get<int>();
                var list = operand1.Get<IEnumerable<IValue>>();
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
