using ANTLR2.ValueBehaviour;
using System;
namespace ANTLR2.Value {
    interface IValue {
        IValue Operator(string op);
        IValue Operator(string op, IValue operand);
        IType Type { get; }

        IValue Constrain(Binding binding);

        T Get<T>();
    }
}
