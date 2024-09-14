using System;

namespace MetaFac.CG5.Expressions
{
    public partial record StringConstantNode
    {
        public static StringConstantNode Create(string value) => new StringConstantNode() { Value = value };
        public static StringConstantNode Create(ReadOnlyMemory<char> source) => new StringConstantNode() { Value = new string(source.Slice(1, source.Length - 2).Span) };
    }
}
