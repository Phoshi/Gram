using ANTLR2.ValueBehaviour;
using System;
namespace ANTLR2.Value {
    interface IValue {
        IValue Operator(string op);
        IValue Operator(string op, IValue operand);
        Type Type { get; }

        IValue Constrain(Type t);

        T Get<T>();
    }
}
