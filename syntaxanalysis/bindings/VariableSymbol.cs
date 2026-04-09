using System;

namespace bindings
{
    internal sealed class VariableSymbol
    {
        public VariableSymbol(string name, Type type, Type? elementType = null)
        {
            Name = name;
            Type = type;
            ElementType = elementType;
        }

        public string Name { get; }
        public Type Type { get; }
        public Type? ElementType { get; }
        public bool IsArray => ElementType is not null;
    }
}
