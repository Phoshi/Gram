using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANTLR2.ValueBehaviour;

namespace ANTLR2.ValueBehaviour {
    class ValueBehaviourFactory {
        public static ValueBehaviour GetBehaviour(IValue val, IValue val2) {
            if (val.Type.RawTypeOf == ValueType.INTEGER && val2.Type.RawTypeOf == ValueType.INTEGER) {
                return new IntBehaviour();
            } else if (val.Type.RawTypeOf == ValueType.LIST && val2.Type.RawTypeOf != ValueType.LIST) {
                if (val2.Type.RawTypeOf == ValueType.INTEGER) {
                    return new ListToIntBehaviour();
                }
                return new ListToOtherBehaviour();
            } else if (val.Type.RawTypeOf == ValueType.LIST && val2.Type.RawTypeOf == ValueType.LIST){
                return new ListBehaviour();
            } else if (val.Type.RawTypeOf == ValueType.FUNCTION){
                return new FuncBehaviour();
            } else if (val.Type.Check(ValueType.TYPE)){
                return new TypeBehaviour();
            } else if (val.Type.Check(ValueType.STRING) && val2.Type.Check(ValueType.STRING)){
                return new StringBehaviour();
            } else if (val.Type.Check(ValueType.STRING) && val2.Type.Check(ValueType.INTEGER)){
                return new StringToIntBehaviour();
            } else {
                return new NullBehaviour();
            }
        }

        public static ValueBehaviour GetBehaviour(IValue val) {
            if (val.Type.RawTypeOf == ValueType.INTEGER) {
                return new IntBehaviour();
            } else if (val.Type.RawTypeOf == ValueType.LIST) {
                return new ListBehaviour();
            } else {
                return new NullBehaviour();
            }
        }
    }
}
