using FluentModels;

namespace MetaFac.CG5.Expressions.Schema
{
    public enum UnaryOperator
    {
        None = 0,
        Plus,       // +x
        Minus,      // -x
        LogicalNot, // not x
        BitwiseNot, // !x
    }
    public enum BinaryOperator
    {
        None = 0,
        Pow = 1, // a ** b
        Add = 2, // a + b
        Sub = 3, // a - b
        Mul = 4, // a * b
        Div = 5, // a / b
        Mod = 6, // a % b
        LSS = 7, // a < b
        LEQ = 8, // a <= b
        GTR = 9, // a > b
        GEQ = 10, // a >= b
        EQU = 11, // a == b
        NEQ = 12, // a != b
        Assign = 13, // a := b
        AND = 14, // a && b
        OR = 15, // a || b
    }
    public enum TertiaryOperator
    {
        None = 0,
        IfThenElse = 1, // x ? a : b
    }

    [Entity(1)]
    public abstract class Node { }

    [Entity(2)]
    public sealed class ErrorNode : Node
    {
        [Member(1)] public string? Message { get; set; }
    }

    [Entity(3)]
    public abstract class ConstantNode : Node { }

    [Entity(4)]
    public sealed class NullConstantNode : ConstantNode { }

    [Entity(5)]
    public sealed class BooleanConstantNode : ConstantNode
    {
        [Member(1)] public bool Value { get; set; }
    }

    [Entity(6)]
    public sealed class StringConstantNode : ConstantNode
    {
        [Member(1)] public string? Value { get; set; }
    }

    [Entity(7)]
    public abstract class NumericConstantNode : ConstantNode { }

    [Entity(8)]
    public sealed class IntegerConstantNode : NumericConstantNode
    {
        [Member(1)] public long Value { get; set; }
    }

    [Entity(9)]
    public sealed class DoubleConstantNode : NumericConstantNode
    {
        [Member(1)] public double Value { get; set; }
    }

    [Entity(10)]
    public sealed class VariableNode : Node
    {
        [Member(1)] public string? Name { get; set; }
    }

    [Entity(11)]
    public abstract class OperatorNode : ConstantNode { }

    [Entity(12)]
    public sealed class BinaryOperatorNode : OperatorNode
    {
        [Member(1)] public BinaryOperator Value { get; set; }
    }

    [Entity(13)]
    public sealed class UnaryExpressionNode : Node
    {
        [Member(1)] public UnaryOperator Op { get; set; }
        [Member(2)] public Node? Operand { get; set; }
    }

    [Entity(14)]
    public sealed class BinaryExpressionNode : Node
    {
        [Member(1)] public BinaryOperator Op { get; set; }
        [Member(2)] public Node? Left { get; set; }
        [Member(3)] public Node? Right { get; set; }
    }

    [Entity(15)]
    public sealed class TertiaryExpressionNode : Node
    {
        [Member(1)] public TertiaryOperator Op { get; set; }
        [Member(2)] public Node? Node1 { get; set; }
        [Member(3)] public Node? Node2 { get; set; }
        [Member(4)] public Node? Node3 { get; set; }
    }
}
