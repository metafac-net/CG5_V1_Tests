namespace MetaFac.CG5.Expressions
{
    public partial record BinaryOperatorNode : OperatorNode
    {
        public static BinaryOperatorNode Create(BinaryOperator value) => new BinaryOperatorNode() { Value = value };
    }
}
