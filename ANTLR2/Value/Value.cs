using ANTLR2.ValueBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class Value : IValue {
        public IType Type { get; set; } 

        private readonly Object value;

        public Value(IType type, Object val = null) {
            if (type.RawTypeOf != ValueType.ANY && val == null) {
                throw new ArgumentNullException();
            }
            value = val;
            Type = type;
        }

        public Value(IType type, IValue val) {
            Type = type;
            value = val.Get<object>();
        }

        public int AsInt { get { return (int)value; } }

        public Func<Value, Value> AsFunc { get { return (Func<Value, Value>)value; } }

        public IEnumerable<IValue> AsList { get { return (IEnumerable<IValue>)value; } }

        public IType AsType { get { return (IType)value; } }

        public T Get<T>() {
            return (T)value;
        }

        public IValue Operator(string op, IValue operand){
            return ValueBehaviourFactory.GetBehaviour(this, operand).BinaryOperator(this, op, operand);
        }

        public IValue Operator(string op) {
            return ValueBehaviourFactory.GetBehaviour(this).UnaryOperator(this, op);
        }

        public IValue Constrain(Binding binding) {
            return new Value(binding.Type, this);
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
            if (Type.RawTypeOf == ValueType.ANY) {
                return "Any";
            }
            return value.ToString();
        }
    }
}
