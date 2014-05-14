using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Type {
        public static Type Of(ValueType rawType) {
            return new Type(rawType);
        }

        public ValueType RawTypeOf { get; internal set; }

        private IValue parentValue;

        public IValue Predicate { get; internal set; }
        private static IValue nopPredicate = ValueFactory.make(x => ValueFactory.make(1));

        private string predicateDescription = "";

        public Type(ValueType type) {
            RawTypeOf = type;
            Predicate = nopPredicate;
        }

        public Type(ValueType type, IValue predicate, string predicateDescription) {
            if (!Type.Of(ValueType.FUNCTION).Check(predicate)) {
                throw new InvalidOperationException("Predicate must be function type");
            }
            RawTypeOf = type;
            Predicate = predicate;
            this.predicateDescription = predicateDescription;
            this.parentValue = null;
        }

        public Type(IValue parent, IValue predicate, string predicateDescription) {
            this.parentValue = parent;
            this.Predicate = predicate;
            this.predicateDescription = predicateDescription;
        }

        public override string ToString() {
            return "Type: " + RawTypeOf + (Predicate == nopPredicate ? "" : "; Predicate: " + predicateDescription);
        }

        public bool Check(IValue val) {
            return val.Type.RawTypeOf == RawTypeOf && Predicate.Operator("()", val).Equals(ValueFactory.make(true));//Predicate.AsFunc(val).AsInt != 0;
        }

        public bool Check(Type type) {
            return type.RawTypeOf == RawTypeOf;// && type.Predicate == Predicate;
        }

        public bool Check(ValueType type) {
            return RawTypeOf == type;
        }

        public override bool Equals(object obj) {
            if (obj is Type) {
                var otherType = obj as Type;
                return Check(otherType);
            }

            return false;
        }

        public override int GetHashCode() {
            return RawTypeOf.GetHashCode();
        }

        public static bool operator ==(Type a, Type b) {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Type a, Type b) {
            return !(a == b);
        }
    }
}
