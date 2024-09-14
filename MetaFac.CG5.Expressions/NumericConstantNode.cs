using System;

namespace MetaFac.CG5.Expressions
{
    public partial record NumericConstantNode
    {
        public static NumericConstantNode Create(long value) => new IntegerConstantNode() { Value = value };
        public static NumericConstantNode Create(double value) => new DoubleConstantNode() { Value = value };
        public static NumericConstantNode Create(ReadOnlyMemory<char> source)
        {
            if (long.TryParse(source.Span, out var longValue))
            {
                return new IntegerConstantNode() { Value = longValue };
            }
            else
                return new DoubleConstantNode() { Value = double.Parse(source.Span) };
        }
    }
}
