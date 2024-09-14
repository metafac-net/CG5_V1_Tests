using OneOf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetaFac.CG5.Parsing
{
    public abstract class Lexer<T> where T : struct
    {
        private readonly List<ITokenMatcher<T>> _matchers;

        protected Lexer(IEnumerable<ITokenMatcher<T>>? matchers)
        {
            _matchers = matchers?.ToList() ?? new List<ITokenMatcher<T>>();
        }

        private OneOf<bool, Token<T>> GetNextToken(ReadOnlyMemory<char> source)
        {
            if (source.Length == 0) return true;

            foreach (var matcher in _matchers)
            {
                (int consumed, Token<T> token) = matcher.Match(source);
                if (consumed > 0)
                {
                    return token;
                }
            }

            return false;
        }

        public IEnumerable<OneOf<Error, Token<T>>> GetTokens(ReadOnlyMemory<char> source)
        {
            int consumed = 0;
            var remaining = source;
            bool final = false;
            bool matched = true;
            while (matched)
            {
                var result = GetNextToken(remaining);
                if (result.TryPickT0(out final, out var token))
                {
                    matched = false;
                }
                else
                {
                    consumed += token.Source.Length;
                    remaining = remaining.Slice(token.Source.Length);
                    yield return token;
                }
            }

            if (!final)
            {
                yield return new Error($"No patterns match text starting at position {consumed}");
            }
        }

        public IEnumerable<Token<T>> GetTokensOnly(ReadOnlyMemory<char> source)
        {
            foreach (var lexerResult in GetTokens(source))
            {
                if (lexerResult.TryPickT0(out var error, out var token))
                {
                    throw new InvalidDataException(error.Message);
                }
                else
                {
                    yield return token;
                }
            }
        }
    }

    public abstract class Parser<TEnum, TNode> where TEnum : struct
    {
        private readonly Lexer<TEnum> _lexer;

        protected Parser(Lexer<TEnum> lexer)
        {
            _lexer = lexer;
        }

        protected abstract bool OnTryMatch(ReadOnlyMemory<Token<TEnum>> tokens, out int consumed, out TNode? result);
        protected abstract IEnumerable<Token<TEnum>> OnFilterTokens(IEnumerable<Token<TEnum>> tokens);
        protected abstract TNode OnMakeErrorNode(string message);

        public TNode Parse(ReadOnlyMemory<char> source)
        {
            var rawTokens = new List<Token<TEnum>>();
            foreach (var lexerResult in _lexer.GetTokens(source))
            {
                if (lexerResult.TryPickT0(out var error, out var token))
                {
                    return OnMakeErrorNode(error.Message);
                }
                else
                {
                    rawTokens.Add(token);
                }
            }

            Token<TEnum>[] tokens = OnFilterTokens(rawTokens).ToArray();

            if (OnTryMatch(tokens, out int consumed, out var result))
            {
                if (consumed != tokens.Length)
                {
                    return OnMakeErrorNode($"Not all source matched. Only {consumed} of {tokens.Length} tokens consumed.");
                }
                else
                {
                    return result!;
                }
            }
            else
            {
                return OnMakeErrorNode($"Parse unsuccessfull. Only {consumed} of {tokens.Length} tokens consumed.");
            }
        }
    }

}
