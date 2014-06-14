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
		public static string preludePath = "example.gram";
		static void Main(string[] args){
			var interpreter = new GramInterpreter();
			interpreter.Execute("local import \"" + preludePath + "\";");

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
