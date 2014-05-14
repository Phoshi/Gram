using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Binding {
        public string Name { get; internal set; }
        public Type Type { get; internal set; }

        public bool ReadOnly { get; internal set; }

        private IValue value = ValueFactory.make();
        public IValue Value { 
            get { return value; }
            set {
                if (Type.Check(ValueType.UNIT)) {
                    Type = value.Type;
                }
                if (ReadOnly && !value.Type.Check(ValueType.UNIT)) {
                    throw new GramException("Cannot reassign 'val'!");
                }
                if (value.Type == Type || Type.Check(value)) {
                    this.value = value.Constrain(Type);
                } else {
                    throw new GramException("Type violation!");
                }
            }
        }

        public Binding(string name) {
            Name = name;
            Type = Type.Of(ValueType.UNIT);
        }

        public Binding(string name, Type type) {
            Name = name;
            Type = type;
        }

        public Binding(string name, IValue val) {
            Name = name;
            Type = val.Type;
            Value = val;
        }

        public Binding(string name, Type type, IValue val) {
            Name = name;
            Type = type;
            Value = val;
        }
    }
}
