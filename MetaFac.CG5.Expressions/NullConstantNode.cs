using System;

namespace MetaFac.CG5.Expressions
{
    // todo use CG4 to generate expression nodes
    public partial record NullConstantNode
    {
        private static readonly NullConstantNode _null = new NullConstantNode();
        public static NullConstantNode Create(ReadOnlyMemory<char> source) => _null;
        public override string ToString() => "null";
    }
}
