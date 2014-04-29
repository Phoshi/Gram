using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
    class Program {
        static void Main(string[] args){
            var visitor = new ExprVisitor();
            while (true) {
                Console.Write("$ ");
                var input = new StreamReader(Console.OpenStandardInput());
                string text = "";
                while (text.LastOrDefault() != ';') {
                    text += input.ReadLine();
                }
                var antlrStream = new AntlrInputStream(text);
                var lexer = new gramLexer(antlrStream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new gramParser(tokens);
                var tree = parser.prog();
                Console.WriteLine("| " + tree.ToStringTree(parser));
                Console.Write("> ");
                try {
                    Value result = visitor.Visit(tree);
                    if (result.Type.RawTypeOf != ValueType.UNIT) {
                        Console.WriteLine(result);
                    }
                    Console.WriteLine();
                } catch (GramException ex) {
                    Console.WriteLine(ex.Message);
                } catch (Exception ex) {
                    Console.WriteLine("Exception at runtime: " + ex.ToString());
                }
            }
        }
    }
}
