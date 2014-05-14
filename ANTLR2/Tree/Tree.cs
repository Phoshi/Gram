using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Tree {
    class Tree<T> {
        private List<Tree<T>> nodes = new List<Tree<T>>();

        private List<T> values;

        public IReadOnlyList<T> Values { get { return values.AsReadOnly();  } }

        public IReadOnlyList<Tree<T>> Children { get { return nodes.AsReadOnly(); } }

        public Tree() {
            values = new List<T>();
        }

        public Tree(T value) {
            values = new List<T>{value};
        }

        public Tree(IEnumerable<T> values) {
            this.values = new List<T>(values);
        }

        public Tree<T> Child(int n){
            return nodes[n];
        }

        public void Add(Tree<T> tree) {
            nodes.Add(tree);
        }

        public IEnumerable<T> GetAllValues() {
            if (values.Count > 0) {
                return Values;
            }
            var vals = new List<T>();
            var allVals = nodes.SelectMany(n => n.GetAllValues());
            vals.AddRange(allVals);
            return vals;
        }
    }
}
