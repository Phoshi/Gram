using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2.ValueBehaviour {
    class ListBehaviour : ValueBehaviour {
        public IValue BinaryOperator(IValue operand1, string op, IValue operand2) {
            switch (op) {
                case "+": {
                    var list = new List<IValue>(operand1.Get<IEnumerable<IValue>>());
                    list.Add(operand2);
                    return ValueFactory.make(list);
                    }
                case "==": {
                    var list = new List<IValue>(operand1.Get<IEnumerable<IValue>>());
                    var otherList = new List<IValue>(operand1.Get<IEnumerable<IValue>>());
                    if (list.Count() != otherList.Count()) {
                        return ValueFactory.make(false);
                    }
                    var results = list.Zip(otherList, (item, otherItem) => item.Operator("==", otherItem));
                    return ValueFactory.make(results.All(r => r.Get<int>() == 1));
                    }
            }

            throw new InvalidOperationException();
        }


        public IValue UnaryOperator(IValue operand1, string op) {
            throw new InvalidOperationException();
        }
    }
}
