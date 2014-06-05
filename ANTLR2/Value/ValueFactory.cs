using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {

    class ValueFactory {
        public static IValue make(int val) {
            return new Value(Type.Of(ValueType.INTEGER), val);
        }

        public static IValue make(bool val) {
            return new Value(Type.Of(ValueType.INTEGER), val ? 1 : 0);
        }

        public static IValue make(Func<int, int> val) {
            Func<IValue, IValue> func = f => {
                return ValueFactory.make(val(f.Get<int>()));
            };
            return new Value(Type.Of(ValueType.FUNCTION), func);
        }

        public static IValue make(Func<IValue, IValue> val, IValue paramType = null, IValue returnType = null) {
            if (paramType == null) paramType = ValueFactory.make(Type.Of(ValueType.ANY));
            if (returnType == null) returnType = ValueFactory.make(Type.Of(ValueType.ANY));
            return new FunctionValue(val, paramType, returnType);
        }

        public static IValue make(IEnumerable<IValue> val) {
            return new Value(Type.Of(ValueType.LIST), val.ToList());
        }

        public static IValue make(IType val) {
            return new TypeValue(val);
        }

        public static IValue make(Environment val) {
            return new Value(Type.Of(ValueType.MODULE), val);
        }

        public static IValue make() {
            return new Value(Type.Of(ValueType.ANY));
        }

    }
}
