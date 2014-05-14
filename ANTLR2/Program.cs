using ANTLR2.Value;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2 {
	class Program {
		public static string prelude = @"   val bool = int<bool=> bool == 0 || bool == 1>;
											val range = p:{int;int}=> range'(p+{p[0]});
											val range' = {current:int; end:int; rangeList} => 
												if (current == end) 
													rangeList 
												else 
													range'({current + 1; end; rangeList + (current+1)});

											var head = list=>list[0];

											var tail = l=>{
												var all = {}; 
												var first = true; 
												for (elem:l) 
													if (first) 
														first = false 
													else 
														all = all + elem;
												all}[-1];

											val map = {f; iter} => for(i:iter) f(i);

											val filter = args=>filter'(args + {});

											val filter' = {f; iter; current} => 
												if (length(iter) == 0) 
													current 
												else if (f(head(iter))) 
													filter'{f; tail iter; current + (head(iter))} 
												else 
													filter'{f; tail iter; current};

											val listConcat = {list1;list2}=>{
												var total = list1;
												for(elem:list2) 
													total = total + elem; 
												total
											}[-1];

											val reduce = {f; iter}=>
												if (length(iter) == 1) 
													head(iter) 
												else 
													reduce{f; 
														listConcat{
															{f{iter[0];iter[1]}}; 
															tail(tail(iter))  }};";
		static void Main(string[] args){
			var visitor = new ExprVisitor();
			var preludeInput = new AntlrInputStream(prelude);
			var preludeLexer = new gramLexer(preludeInput);
			var preludeTokens = new CommonTokenStream(preludeLexer);
			var preludeParser = new gramParser(preludeTokens);
			visitor.Visit(preludeParser.prog());

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
					IValue result = visitor.Visit(tree);
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
