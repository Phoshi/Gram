using ANTLR2.Value;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Interpret {
    public class GramInterpreter {
        private readonly ExprVisitor interpreter;

        public GramInterpreter() {
            interpreter = new ExprVisitor();
        }

        private gramParser getParser(string code){
            var input = new AntlrInputStream(code);
            var lexer = new gramLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new gramParser(tokens);
            return parser;
        }

        public IValue Execute(string code) {
            var parser = getParser(code);

            return interpreter.Visit(parser.prog());
        }

        public string GetParseTree(string code) {
            var parser = getParser(code);
            return parser.prog().ToStringTree(parser);
        }

        public IValue GetVariable(string name) {
            return interpreter.Environment[name].Value;
        }
    }
}
