using System;

namespace MetaFac.CG5.Parsing
{
    public readonly struct Token<TEnum> where TEnum : struct
    {
        public readonly TEnum Kind;
        public readonly ReadOnlyMemory<char> Source;

        public Token(TEnum kind, ReadOnlyMemory<char> source) : this()
        {
            Kind = kind;
            Source = source;
        }
    }
}