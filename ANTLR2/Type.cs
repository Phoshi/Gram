using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Type {
        public ValueType TypeOf { get; internal set; }

        public Type(ValueType type) {
            TypeOf = type;
        }

        public override string ToString() {
            return "Type: " + TypeOf;
        }
    }
}
