using System;

namespace MetaFac.CG5.Parsing
{
    public interface ITokenMatcher<TEnum> where TEnum : struct
    {
        (int, Token<TEnum>) Match(ReadOnlyMemory<char> source);
    }
}
