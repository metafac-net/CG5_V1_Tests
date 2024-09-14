namespace MetaFac.CG5.Expressions
{
    public partial record VariableNode
    {
        public static VariableNode Create(string name) => new VariableNode() { Name = name };
        public override string ToString() => Name ?? "_no_name_";
    }
}
