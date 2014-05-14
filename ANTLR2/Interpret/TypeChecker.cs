using ANTLR2.Tree;
using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Interpret {
    class TypeChecker {
        private readonly IValue typeTree;
        public TypeChecker(IValue typeTree) {
            this.typeTree = typeTree;
        }

        public bool Check(IValue value) {
            if (typeTree.Type.Check(ValueType.TYPE) && typeTree.Get<Type>().Check(ValueType.UNIT)) {
                return true;
            }
            if (!value.Type.Check(ValueType.LIST) || !typeTree.Type.Check(ValueType.LIST)) {
                return typeTree.Get<Type>().Check(value);
            } else {
                return typeTree.Get<IEnumerable<IValue>>().Zip(value.Get<IEnumerable<IValue>>(), (bindChildren, val) => new TypeChecker(bindChildren).Check(val)).Aggregate((one, two) => one && two);
            }
        }
    }
}
