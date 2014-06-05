using ANTLR2.Interpret;
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
		public static string prelude = @"   val Bool = Int<bool=> bool == 0 || bool == 1>;
											val true: Bool = 1;
											val false: Bool = 0;
											val range = let val range' = {current:Int; end:Int; rangeList} => 
													if (current == end) 
														rangeList 
													else 
														range'({current + 1; end; rangeList + (current+1)})
												in p:{Int;Int} => range'(p+{p[0]});

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

											val filter: {Any->Any; Any} -> Any = let val filter' = {f; iter; current} => 
													if (length(iter) == 0) 
														current 
													else if (f(head(iter))) 
														filter'{f; tail iter; current + (head(iter))} 
													else 
														filter'{f; tail iter; current}
												in args=>filter'(args + {});

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
			var interpreter = new GramInterpreter();
			interpreter.Execute(prelude);

			while (true) {
				Console.Write("$ ");
				var input = new StreamReader(Console.OpenStandardInput());
				string text = input.ReadLine();
				while (text.LastOrDefault() != ';') {
                    Console.Write("...\t");
					text += input.ReadLine();
				}
				Console.WriteLine("| " + interpreter.GetParseTree(text));
				Console.Write("> ");
				try {
					IValue result = interpreter.Execute(text);
					if (result.Type.RawTypeOf != ValueType.ANY) {
						Console.WriteLine(result + ": " + result.Type);
					}
					Console.WriteLine();
				} catch (GramException ex) {
					Console.WriteLine(ex.GetType().FullName + ": " + ex.Message);
				} catch (Exception ex) {
					Console.WriteLine("Exception at runtime: " + ex.ToString());
				}
			}
		}
	}
}
