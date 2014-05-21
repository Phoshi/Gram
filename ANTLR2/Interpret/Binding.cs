using ANTLR2.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    public class Binding {
        public string Name { get; internal set; }
        public IType Type { get; internal set; }

        public bool ReadOnly { get; internal set; }

        private IValue value = ValueFactory.make();
        public IValue Value { 
            get { return value; }
            set {
                if (Type.Check(ValueType.ANY)) {
                    Type = value.Type;
                }
                if (ReadOnly && !value.Type.Check(ValueType.ANY)) {
                    throw new GramException("Cannot reassign 'val'!");
                }
                if (value.Type == Type || Type.Check(value)) {
                    this.value = value.Constrain(this);
                } else {
                    this.value = value.Constrain(this);
                    throw new TypeException(Name + " has type " + Type.ToString());
                }
            }
        }

        public Binding(string name) {
            Name = name;
            Type = new Type(ValueType.ANY);
        }

        public Binding(string name, IType type) {
            Name = name;
            Type = type;
        }

        public Binding(string name, IValue val) {
            Name = name;
            Type = val.Type;
            Value = val;
        }

        public Binding(string name, IType type, IValue val) {
            Name = name;
            Type = type;
            Value = val;
        }
    }
}
