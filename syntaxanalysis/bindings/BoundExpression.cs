using System;

namespace bindings
{
    internal abstract class Boundexpression : Boundnode
    {
        public abstract Type Type { get; }
    }
}
