using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Value {
        public Type Type { get; private set; } 

        private readonly Object value;

        public Value(Type type, Object val = null) {
            if (type.RawTypeOf != ValueType.UNIT && val == null) {
                throw new ArgumentNullException();
            }
            value = val;
            Type = type;
        }

        public int AsInt { get { return (int)value; } }

        public Func<Value, Value> AsFunc { get { return (Func<Value, Value>)value; } }

        public IEnumerable<Value> AsList { get { return (IEnumerable<Value>)value; } }

        public Type AsType { get { return (Type)value; } }

        public static Value operator +(Value a, Value b){
            return ValueBehaviourFactory.GetBehaviour(a, b).BinaryOperator(a, "+", b);
        }

        public static Value operator -(Value a, Value b) {
            return ValueBehaviourFactory.GetBehaviour(a, b).BinaryOperator(a, "-", b);
        }

        public static Value operator *(Value a, Value b) {
            return ValueBehaviourFactory.GetBehaviour(a, b).BinaryOperator(a, "*", b);
        }

        public static Value operator /(Value a, Value b) {
            return ValueBehaviourFactory.GetBehaviour(a, b).BinaryOperator(a, "/", b);
        }

        public override bool Equals(object obj) {
            if (obj is Value) {
                var val2 = obj as Value;
                return ValueBehaviourFactory.GetBehaviour(this, val2).BinaryOperator(this, "==", val2).AsInt == 1;
            }
            return false;
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }

        public override string ToString() {
            if (Type.RawTypeOf == ValueType.LIST) {
                return "{" + string.Join("; ", (value as IEnumerable<Value>).Select(x=>x.ToString())) + "}";
            }
            if (Type.RawTypeOf == ValueType.UNIT) {
                return "Unit";
            }
            if (Type.RawTypeOf == ValueType.FUNCTION) {
                return Type.ToString();
            }
            return value.ToString();
        }
    }

    class ValueFactory {
        public static Value make(int val) {
            return new Value(Type.Of(ValueType.INTEGER), val);
        }

        public static Value make(Func<int, int> val) {
            Func<Value, Value> func = f => { 
                return ValueFactory.make(val(f.AsInt)); 
            };
            return new Value(Type.Of(ValueType.FUNCTION), func);
        }

        public static Value make(Func<Value, Value> val) {
            return new Value(Type.Of(ValueType.FUNCTION), val);
        }

        public static Value make(IEnumerable<Value> val) {
            return new Value(Type.Of(ValueType.LIST), val.ToList());
        }

        public static Value make(Type val) {
            return new Value(Type.Of(ValueType.TYPE), val);
        }

        public static Value make() {
            return new Value(Type.Of(ValueType.UNIT));
        }

    }
}
