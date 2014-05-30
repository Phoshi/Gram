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
        private readonly bool compareAsType;
        public TypeChecker(IValue typeTree, bool compareAsType = false) {
            this.typeTree = typeTree;
            this.compareAsType = compareAsType;
        }

        public bool Check(IValue value) {
            if (typeTree.Type.Check(ValueType.TYPE) && typeTree.Get<IType>().Check(ValueType.ANY)) {
                return true;
            }
            if (value.Type.Check(ValueType.TYPE) && value.Get<IType>().Check(ValueType.ANY)) {
                return true;
            }
            if (!value.Type.Check(ValueType.LIST) || !typeTree.Type.Check(ValueType.LIST)) {
                if (compareAsType) {
                    if (value.Type.Check(ValueType.LIST)) {
                        return typeTree.Get<IType>().Check(new ListType(value, value.ToString()));
                    }
                    return typeTree.Get<IType>().Check(value.Get<IType>());
                }
                return typeTree.Get<IType>().Check(value);

            } else {
                return typeTree.Get<IEnumerable<IValue>>().Zip(value.Get<IEnumerable<IValue>>(), (bindChildren, val) => new TypeChecker(bindChildren, compareAsType).Check(val)).Aggregate((one, two) => one && two);
            }
        }
    }
}
