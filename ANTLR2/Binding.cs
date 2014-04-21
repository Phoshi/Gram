using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Binding {
        public string Name { get; internal set; }
        public ValueType Type { get; internal set; }

        public bool ReadOnly { get; internal set; }

        private Value value = ValueFactory.make();
        public Value Value { 
            get { return value; }
            set {
                if (ReadOnly) {
                    throw new GramException("Cannot reassign 'val'!");
                }
                if (value.Type == Type) {
                    this.value = value;
                } else {
                    throw new GramException("Type violation!");
                }
            }
        }

        public Binding(string name, Value val) {
            Name = name;
            Type = val.Type;
            Value = val;
        }
    }
}
