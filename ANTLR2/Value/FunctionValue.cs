using ANTLR2.Interpret;
using ANTLR2.ValueBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTLR2.Value {
    class FunctionValue : IValue {
        private readonly Func<IValue, IValue> innerFunction;
        private readonly IValue parameterType;
        private readonly IValue returnType;

        public FunctionValue(Func<IValue, IValue> func) {
            this.innerFunction = func;
            this.parameterType = ValueFactory.make(new Type(ValueType.ANY));
            this.returnType = ValueFactory.make(new Type(ValueType.ANY));
        }
        public FunctionValue(Func<IValue, IValue> func, IValue parameterType) {
            this.innerFunction = func;
            this.parameterType = parameterType;
            this.returnType = ValueFactory.make(new Type(ValueType.ANY));
        }
        public FunctionValue(Func<IValue, IValue> func, IValue parameterType, IValue returnType) {
            this.innerFunction = func;
            this.parameterType = parameterType;
            this.returnType = returnType;
        }
        public IValue Operator(string op) {
            return ValueBehaviourFactory.GetBehaviour(this).UnaryOperator(this, op);
        }

        public IValue Operator(string op, IValue operand) {
            if (op == "()") {
                var parameterChecker = new TypeChecker(parameterType);
                if (!parameterChecker.Check(operand)) {
                    throw new TypeException("Function parameter is invalid");
                }
                var result = ValueBehaviourFactory.GetBehaviour(this, operand).BinaryOperator(this, op, operand);
                var resultChecker = new TypeChecker(returnType);
                if (!resultChecker.Check(result)) {
                    throw new TypeException("Function result is invalid");
                }
                IType type;
                if (returnType.Type.Check(ValueType.TYPE)) {
                    type = returnType.Get<IType>();
                } else if (returnType.Type.Check(ValueType.LIST)) {
                    type = new ListType(returnType, returnType.ToString());
                } else {
                    throw new TypeException("Function return value invalid!");
                }
                return new Binding(result.ToString(), type, result).Value;
            }
            return ValueBehaviourFactory.GetBehaviour(this, operand).BinaryOperator(this, op, operand);
        }

        public IType Type {
            get { return new FunctionType(parameterType, returnType); }
            set { }
        }

        public IValue Constrain(Binding t) {
            if (t.Type is FunctionType) {
                var bindType = t.Type as FunctionType;
                return new FunctionValue(innerFunction, bindType.Parameter, bindType.Return);
            }
            return this;
        }

        public T Get<T>() {
            return (T)(object)innerFunction;
        }

        public override string ToString() {
            return parameterType.ToString() + "=>" + returnType.ToString();
        }
    }
}
