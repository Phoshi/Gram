using ANTLR2.ValueBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class Value : IValue {
        public Type Type { get; set; } 

        private readonly Object value;

        public Value(Type type, Object val = null) {
            if (type.RawTypeOf != ValueType.UNIT && val == null) {
                throw new ArgumentNullException();
            }
            value = val;
            Type = type;
        }

        public Value(Type type, IValue val) {
            Type = type;
            value = val.Get<object>();
        }

        public int AsInt { get { return (int)value; } }

        public Func<Value, Value> AsFunc { get { return (Func<Value, Value>)value; } }

        public IEnumerable<IValue> AsList { get { return (IEnumerable<IValue>)value; } }

        public Type AsType { get { return (Type)value; } }

        public T Get<T>() {
            return (T)value;
        }

        public IValue Operator(string op, IValue operand){
            return ValueBehaviourFactory.GetBehaviour(this, operand).BinaryOperator(this, op, operand);
        }

        public IValue Operator(string op) {
            return ValueBehaviourFactory.GetBehaviour(this).UnaryOperator(this, op);
        }

        public IValue Constrain(Type t) {
            return new Value(t, this);
        }

        public override bool Equals(object obj) {
            if (obj is Value) {
                var val2 = obj as Value;
                return (ValueBehaviourFactory.GetBehaviour(this, val2).BinaryOperator(this, "==", val2)).Get<int>() == 1;
            }
            return false;
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }

        public override string ToString() {
            if (Type.RawTypeOf == ValueType.LIST) {
                return "{" + string.Join("; ", (value as IEnumerable<IValue>).Select(x=>x.ToString())) + "}";
            }
            if (Type.RawTypeOf == ValueType.UNIT) {
                return "Unit";
            }
            return value.ToString();
        }
    }

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

        public static IValue make(Func<IValue, IValue> val) {
            return new FunctionValue(val);
        }

        public static IValue make(IEnumerable<IValue> val) {
            return new Value(Type.Of(ValueType.LIST), val.ToList());
        }

        public static IValue make(Type val) {
            return new Value(Type.Of(ValueType.TYPE), val);
        }

        public static IValue make() {
            return new Value(Type.Of(ValueType.UNIT));
        }

    }
}
