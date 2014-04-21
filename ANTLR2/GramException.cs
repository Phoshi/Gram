using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANTLR2 {
    class GramException : Exception {
        public GramException(string message) : base(message){}
    }
}
