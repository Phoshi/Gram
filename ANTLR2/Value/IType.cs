using System;
namespace ANTLR2.Value {
    public interface IType {
        bool Check(IType type);
        bool Check(IValue val);
        bool Check(ANTLR2.ValueType type);
        string FriendlyName { get; set; }

        ValueType RawTypeOf { get; }
        IValue Predicate { get; }
    }
}
