namespace MetaFac.CG5.Parsing
{
    public interface IParser<TEnum, TNode> where TEnum : struct
    {
        ParserResult<TNode> Match(ParserInputs<TEnum> inputs);
    }

}
