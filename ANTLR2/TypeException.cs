using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    public class TypeException : GramException {
        public TypeException(string message) : base(message) { }
    }
}
