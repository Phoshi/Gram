using ANTLR2.Interpret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class ListType : IType {

        private IValue predicate;
        private String friendlyName;

        private IValue creationList;

        public ListType(IEnumerable<IValue> types, String description) {
            this.predicate = ValueFactory.make(makeListTypeChecker(types));
            this.friendlyName = description;
            this.creationList = ValueFactory.make(types);
        }

        public ListType(IValue types, String description) {
            if (!types.Type.Check(ValueType.LIST)) {
                throw new TypeException("List type must be actually a list!");
            }
            this.predicate = ValueFactory.make(makeListTypeChecker(types.Get<IEnumerable<IValue>>()));
            this.friendlyName = description;
            this.creationList = types;
        }

        private Func<IValue, IValue> makeListTypeChecker(IEnumerable<IValue> types) {
            Func<IValue, IValue> typeChecker = l => {
                var typeCheck = new TypeChecker(ValueFactory.make(types));
                return ValueFactory.make(typeCheck.Check(l));
            };
            return typeChecker;
        }

        public bool Check(IType type) {
            if (type is ListType) {
                var otherList = type as ListType;
                return creationList.Operator("==", otherList.creationList).Get<int>() == 1;
            }
            return RawTypeOf == ValueType.ANY || (type.RawTypeOf == RawTypeOf && type.Predicate == Predicate);            
        }

        public bool Check(IValue val) {
            return Check(val.Type) || (val.Type.RawTypeOf == RawTypeOf && Predicate.Operator("()", val).Equals(ValueFactory.make(true)));

        }

        public bool Check(ValueType type) {
            return type == RawTypeOf || RawTypeOf == ValueType.ANY;
        }

        public string FriendlyName {
            get {
                return friendlyName;
            }
            set {
                friendlyName = value;
            }
        }

        public ValueType RawTypeOf {
            get { return ValueType.LIST; }
        }

        public IValue Predicate {
            get { return predicate; }
        }

        public override string ToString() {
            return friendlyName;
        }
    }
}
