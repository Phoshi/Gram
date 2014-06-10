using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ANTLR2.Interpret;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GramTests {
    [TestClass]
    public class Basics {

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.GramException), "Readonly assignment could be reassigned")]
        public void Assignment_Readonly() {
            var interpreter = new GramInterpreter();
            interpreter.Execute("val x = 5;");
            Assert.AreEqual(5, interpreter.GetVariable("x").Get<int>(), "Readonly assignment broken.");
            try {
                interpreter.Execute("x = 6;");
            } catch (ANTLR2.GramException e) {
                Assert.AreEqual(5, interpreter.GetVariable("x").Get<int>(), "Readonly variable was reassigned!");
                throw e;
            }
            
        }

        [TestMethod]
        public void Assignment() {
            var interpreter = new GramInterpreter();
            interpreter.Execute("var x = 5;");
            Assert.AreEqual(5, interpreter.GetVariable("x").Get<int>(), "Basic assignment is broken.");
            interpreter.Execute("x = 6");
            Assert.AreEqual(6, interpreter.GetVariable("x").Get<int>(), "Reassigning variables is broken");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void ReassignType() {
            var interpreter = new GramInterpreter();
            interpreter.Execute("var x: Int = 3;");
            interpreter.Execute("x = Int;");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void ListType() {
            var interpreter = new GramInterpreter();
            try {
                interpreter.Execute("var x: {Int; Int} = {0;1};");
            } catch (ANTLR2.TypeException) {
                Assert.Fail("Initial assignment failed");
            }
            interpreter.Execute("x = {Int; Int};");
        }

        [TestMethod]
        public void Arithmetic() {
            var i = new GramInterpreter();
            Assert.AreEqual(4, i.Execute("2+2;").Get<int>(), "Addition is broken");
            Assert.AreEqual(5, i.Execute("6-1;").Get<int>(), "Subtraction is broken");
            Assert.AreEqual(5, i.Execute("10/2;").Get<int>(), "Division is broken");
            Assert.AreEqual(9, i.Execute("3*3;").Get<int>(), "Multiplication is broken");
        }

        [TestMethod]
        public void If() {
            var i = new GramInterpreter();
            var trueVal = i.Execute("if (1) 5 else 10");
            Assert.AreEqual(5, trueVal.Get<int>(), "If true path not followed");

            var falseVal = i.Execute("if (1) 5 else 10");
            Assert.AreEqual(5, falseVal.Get<int>(), "If false path not taken");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void InvalidIfPredicateType() {
            var i = new GramInterpreter();
            i.Execute("if (Int) 5");
        }

        [TestMethod]
        public void For() {
            var i = new GramInterpreter();
            i.Execute("var total = 0;");
            i.Execute("for (num : {0;1;2;3}) total = total + num;");
            Assert.AreEqual(6, i.GetVariable("total").Get<int>(), "For loop not iterating correctly");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void InvalidForIterableType() {
            var i = new GramInterpreter();
            i.Execute("for (x: 5) 5");
        }

        [TestMethod]
        public void While() {
            var i = new GramInterpreter();
            i.Execute("var total = 0;");
            i.Execute("var count = 3;");
            i.Execute("while (count > 0) {total = total + count; count = count - 1;}");
            Assert.AreEqual(6, i.GetVariable("total").Get<int>(), "While loop not iterating correctly.");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void InvalidWhilePredicateType() {
            var i = new GramInterpreter();
            i.Execute("while (Int) 3;");
        }

        [TestMethod]
        public void LogicalOperators() {
            var i = new GramInterpreter();
            var andTrue = i.Execute("1&&1");
            var andFalse = i.Execute("1&&0");

            var orTrue = i.Execute("1||0");
            var orFalse = i.Execute("0||0");

            Assert.AreEqual(1, andTrue.Get<int>());
            Assert.AreEqual(0, andFalse.Get<int>());
            Assert.AreEqual(1, orTrue.Get<int>());
            Assert.AreEqual(0, orFalse.Get<int>());
        }

        [TestMethod]
        public void FunctionLiteral() {
            var i = new GramInterpreter();
            var func = i.Execute("anyVal => 0");
            Assert.IsTrue(func.Type.Check(ANTLR2.ValueType.FUNCTION), "Function literal not defining.");
        }

        [TestMethod]
        public void Variable() {
            var i = new GramInterpreter();
            i.Execute("val x = 6;");
            Assert.AreEqual(6, i.Execute("x").Get<int>(), "The binding cannot hold");
        }

        [TestMethod]
        public void FunctionCall() {
            var i = new GramInterpreter();
            var result = i.Execute("(x=>x*2)(3);");
            Assert.AreEqual(6, result.Get<int>(), "Function calls are broken");
        }

        [TestMethod]
        public void List() {
            var i = new GramInterpreter();
            Assert.IsTrue(i.Execute("{1;1;1}").Type.Check(ANTLR2.ValueType.LIST), "List syntax isn't returning a list");
        }

        [TestMethod]
        public void ListIndex() {
            var i = new GramInterpreter();
            Assert.AreEqual(6, i.Execute("{1;2;3;4;5;6}[-1]").Get<int>(), "List indexing is broken");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void InvalidListIndexList() {
            var i = new GramInterpreter();
            i.Execute("5[1];");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void InvalidListIndexIndex() {
            var i = new GramInterpreter();
            i.Execute("{1;2}[Int];");
        }

        [TestMethod]
        public void DestructuringAssignment() {
            var i = new GramInterpreter();
            i.Execute("var {a; b; {c; {d; e}; {f; g}}} = {1; 2; {3; {4; 5}; {6; 7}}};");
            var resultMap = new Dictionary<char, int>{
                {'a', 1},
                {'b', 2},
                {'c', 3},
                {'d', 4},
                {'e', 5},
                {'f', 6},
                {'g', 7},
            };
            Assert.IsTrue(resultMap.All(kvP => i.GetVariable(kvP.Key.ToString()).Get<int>() == kvP.Value), "Destructuring assignment doesn't work");
        }

        [TestMethod]
        public void TypeLiteral() {
            var i = new GramInterpreter();
            Assert.IsTrue(i.Execute("Int<i=>i==0>;").Type.Check(ANTLR2.ValueType.TYPE), "Function literals work");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void PredicatedType() {
            var i = new GramInterpreter();
            try {
                i.Execute("var x: Int<x=>x>0> = 1;");
            } catch (ANTLR2.TypeException) {
                Assert.Fail("Initial assignment had invalid type");
            }
            i.Execute("x = -2");
        }

        [TestMethod]
        public void ListAsType() {
            var i = new GramInterpreter();
            i.Execute("val Rect = {{Int; Int}; {Int; Int}};");
            i.Execute("val r: Rect = {{0;0}; {10;10}};");
        }

        [TestMethod]
        public void FunctionType() {
            var i = new GramInterpreter();
            i.Execute("var f: Int->Int = x=>x;");
            Assert.AreEqual(5, i.Execute("f 5").Get<int>(), "Base types not working");
        }

        [TestMethod]
        public void FunctionDestrucuringType() {
            var i = new GramInterpreter();
            i.Execute("var Rect = {Int; Int; Int; Int};");
            i.Execute("var mkSquare: {Int; Int; Int}->Rect = {x: Int; y: Int; lengths: Int}=>{x;y;lengths;lengths};");
            Assert.AreEqual(5, i.Execute("mkSquare({0;0;5})[3];").Get<int>());
        }

        [TestMethod]
        public void FunctionLateType() {
            var i = new GramInterpreter();
            i.Execute("var Rect = {Int; Int; Int; Int};");
            i.Execute("var mkSquare = {x: Int; y: Int; length}=>{x;y;length;length};");
            Assert.AreEqual(5, i.Execute("mkSquare({0;0;5})[3];").Get<int>());
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void FailedFunctionType() {
            var i = new GramInterpreter();
            i.Execute("var Rect = {Int; Int; Int; Int};");
            i.Execute("var embiggen: Rect->Rect = r:Rect->Int=>3;");
        }

        [TestMethod]
        public void Comment() {
            var i = new GramInterpreter();
            i.Execute("var x = 0; #{set x to 5}# x = 5;");
            Assert.AreEqual(5, i.GetVariable("x").Get<int>());
        }

        [TestMethod]
        public void TypeInstantiation() {
            var i = new GramInterpreter();
            i.Execute("var x = Int(5);");
            Assert.AreEqual(5, i.GetVariable("x").Get<int>());

            i.Execute("var Rect = {Int; Int; Int; Int};");
            i.Execute("var box: Rect = Rect{0; 0; 10; 10};");
        }

        [TestMethod]
        [ExpectedException(typeof(ANTLR2.TypeException))]
        public void IncorrectTypeInstantiation() {
            var i = new GramInterpreter();
            i.Execute("var Rect = {Int; Int; Int; Int};");
            i.Execute("var box: Rect = Rect{0; 0; 10};");
        }

        [TestMethod]
        public void MiniModule() {
            var i = new GramInterpreter();
            i.Execute("var mod = module {val x = 5};");
            Assert.AreEqual(5, i.Execute("mod::x").Get<int>());
        }

        [TestMethod]
        public void ModuleValueInFunctionType() {
            var i = new GramInterpreter();
            i.Execute("var mod = module val T = Int;");
            i.Execute("var f: mod::T->mod::T = x=>x");
            Assert.AreEqual(5, i.Execute("f 5").Get<int>());
        }

        [TestMethod]
        public void Let() {
            var i = new GramInterpreter();
            Assert.AreEqual(5, i.Execute("let val x = 5 in x;").Get<int>());
        }

        [TestMethod]
        public void PatternMatch() {
            var i = new GramInterpreter();
            Assert.AreEqual(5, i.Execute("5 match {x=>x};").Get<int>(), "Basic syntax failure");

            Assert.AreEqual(5, i.Execute("5 match {x: Type=>3; x: Int => 5};").Get<int>(), "Typecheck failure");

            Assert.AreEqual(5, i.Execute("2 match {x: Int<(x=>x<4)> => x+3; x=>0}").Get<int>(), "Constrained typecheck failure");
        }

        [TestMethod]
        public void StringType() {
            var i = new GramInterpreter();
            i.Execute("val str: String = \"Hello, World!\";");
            var stdout = new StringWriter();
            Console.SetOut(stdout);
            i.Execute("print(str);");

            Assert.AreEqual("Hello, World!\r\n", stdout.ToString());

            i.Execute("val hello = \"Hello\";   val world = \"World\";");

            stdout = new StringWriter();
            Console.SetOut(stdout);
            i.Execute("print(hello + \", \" + world + \"!\");");

            Assert.AreEqual("Hello, World!\r\n", stdout.ToString());

            stdout = new StringWriter();
            Console.SetOut(stdout);
            i.Execute("print(hello[3]);");
            Assert.AreEqual("l\r\n", stdout.ToString());
        }
    }
}
