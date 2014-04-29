using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class ValueBehaviourFactory {
        public static ValueBehaviour GetBehaviour(Value val, Value val2) {
            if (val.Type.RawTypeOf == ValueType.INTEGER && val2.Type.RawTypeOf == ValueType.INTEGER) {
                return new IntBehaviour();
            } else if (val.Type.RawTypeOf == ValueType.LIST && val2.Type.RawTypeOf != ValueType.LIST) {
                return new ListToOtherBehaviour();
            } else if (val.Type.RawTypeOf == ValueType.LIST && val2.Type.RawTypeOf == ValueType.LIST){
                return new ListBehaviour();
            } else {
                return new NullBehaviour();
            }
        }
    }
}
