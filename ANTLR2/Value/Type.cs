using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Type : ANTLR2.Value.IType {
        public static IType Of(ValueType rawType) {
            return new Type(rawType);
        }

        public ValueType RawTypeOf { get; internal set; }

        private IType parentValue;

        public IValue Predicate { get; internal set; }
        public static IValue NoOpPredicate = ValueFactory.make(x => ValueFactory.make(1));

        private string predicateDescription = "";

        public Type(ValueType type) {
            RawTypeOf = type;
            Predicate = NoOpPredicate;
            parentValue = null;
        }

        public Type(IValue parent) {
            this.parentValue = parent.Get<IType>();
            Predicate = NoOpPredicate;
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
            this.parentValue = parent.Get<IType>();
            this.Predicate = predicate;
            this.predicateDescription = predicateDescription;
        }

        public String FriendlyName { get; set; }

        public override string ToString() {
            if (!String.IsNullOrWhiteSpace(FriendlyName)) {
                return FriendlyName;
            }
            return RawTypeOf + (Predicate == NoOpPredicate ? "" : "<" + predicateDescription + ">");
        }

        public bool Check(IValue val) {
            return Check(val.Type) || ((parentValue == null ? true : parentValue.Check(val)) && val.Type.RawTypeOf == RawTypeOf && Predicate.Operator("()", val).Equals(ValueFactory.make(true)));
        }

        public bool Check(IType type) {
            return RawTypeOf == ValueType.ANY || (type.RawTypeOf == RawTypeOf && type.Predicate == Predicate);
        }

        public bool Check(ValueType type) {
            return RawTypeOf == type || RawTypeOf == ValueType.ANY;
        }

        public override bool Equals(object obj) {
            if (obj is IType) {
                var otherType = obj as IType;
                return Check(otherType);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return RawTypeOf.GetHashCode();
        }

        public static bool operator ==(Type a, Type b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }
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
