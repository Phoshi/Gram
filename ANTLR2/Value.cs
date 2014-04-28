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
            if (a.Type.RawTypeOf == ValueType.INTEGER && b.Type.RawTypeOf == ValueType.INTEGER){
                return ValueFactory.make(a.AsInt + b.AsInt);
            }
            if (a.Type.RawTypeOf == ValueType.LIST) {
                var list = new List<Value>(a.AsList);
                list.Add(b);
                return ValueFactory.make(list);
            }

            throw new GramException(a.Type + "+" + b.Type + " has no known operator+");
        }

        public static Value operator -(Value a, Value b) {
            return ValueFactory.make(a.AsInt - b.AsInt);
        }

        public static Value operator *(Value a, Value b) {
            return ValueFactory.make(a.AsInt * b.AsInt);
        }

        public static Value operator /(Value a, Value b) {
            return ValueFactory.make(a.AsInt / b.AsInt);
        }

        public override bool Equals(object obj) {
            if (obj is Value){
                switch (Type.RawTypeOf) { 
                    case ValueType.INTEGER:
                        return AsInt == (obj as Value).AsInt;
                    case ValueType.FUNCTION:
                        return AsFunc == (obj as Value).AsFunc;
                    case ValueType.LIST:
                        return AsList == (obj as Value).AsList;
                    case ValueType.TYPE:
                        return AsType == (obj as Value).AsType;
                }
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
