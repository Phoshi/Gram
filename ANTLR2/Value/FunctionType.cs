using ANTLR2.Interpret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class FunctionType : IType {
        private readonly IValue param;
        private readonly IValue ret;

        public IValue Parameter { get { return param; } }
        public IValue Return { get { return ret; } }

        private string friendlyName;
        public FunctionType(IValue parameterType, IValue returnType) {
            param = parameterType;
            ret = returnType;
            friendlyName = parameterType + "->" + returnType;
        }
        public bool Check(IType type) {
            return this.Equals(type);
        }

        public bool Check(IValue val) {
            return this.Equals(val.Type);
        }

        public bool Check(ValueType type) {
            return type == RawTypeOf;
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
            get { return ValueType.FUNCTION; }
        }

        public IValue Predicate {
            get { return Type.NoOpPredicate; }
        }

        public override bool Equals(object obj) {
            if (obj is FunctionType) {
                var funcType = obj as FunctionType;

                var paramMatches = ret == funcType.ret || ret.Get<IType>().Check(ValueType.ANY) || funcType.ret.Get<IType>().Check(ValueType.ANY);
                var returnMatches = ret == funcType.ret || ret.Get<IType>().Check(ValueType.ANY) || funcType.ret.Get<IType>().Check(ValueType.ANY);
                return paramMatches && returnMatches;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return param.GetHashCode() + ret.GetHashCode();
        }

        public override string ToString() {
            return FriendlyName;
        }
    }
}
