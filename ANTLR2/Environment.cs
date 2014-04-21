using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Environment : IEnumerable<Binding> {
        private Environment parent;

        private Dictionary<string, Binding> bindings = new Dictionary<string, Binding>();

        public Environment() { }
        public Environment(Environment parent) {
            this.parent = parent;
        }

        public Environment GetChildEnvironment() {
            return new Environment(this);
        }


        public Binding this[string bindname] {
            get { 
                if (bindings.ContainsKey(bindname)){
                    return bindings[bindname];  
                } else if (parent != null){
                    return parent[bindname];
                } else {
                    throw new GramException("No such variable in scope!");
                }
            }
            set {
                if (bindings.ContainsKey(bindname)) {
                    throw new GramException("Cannot overwrite binding in the same scope!");
                }
                bindings[bindname] = value; 
            }
        }

        public IEnumerator<Binding> GetEnumerator() {
            return bindings.Values.GetEnumerator();
        }

        public void Add(Binding binding) {
            this[binding.Name] = binding;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return bindings.Values.GetEnumerator();
        }
    }
}
