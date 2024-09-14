using System;

namespace MetaFac.CG5.Expressions
{
    public partial record BooleanConstantNode : ConstantNode
    {
        private static readonly BooleanConstantNode _true = new BooleanConstantNode() { Value = true };
        private static readonly BooleanConstantNode _false = new BooleanConstantNode() { Value = false };
        public static BooleanConstantNode Create(ReadOnlyMemory<char> source) => bool.Parse(source.Span) ? _true : _false;
        public override string ToString() => Value ? "true" : "false";
    }
}
