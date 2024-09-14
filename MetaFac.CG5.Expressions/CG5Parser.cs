using MetaFac.CG5.Parsing;
using System;
using System.Collections.Generic;

namespace MetaFac.CG5.Expressions
{
    public sealed class CG5Parser : Parser<CG5Token, Node>
    {
        // -------------------- start of grammar --------------------------------------
        private static ParserResult<Node> MatchSymbol(ParserInputs<CG5Token> input, CG5Token symbol)
        {
            var tokens = input.Source.Span;
            if (tokens.Length == 0) return default;
            if (tokens[0].Kind != symbol) return default;
            return new ParserResult<Node>(1, SymbolConstantNode.Create(tokens[0]));
        }

        private static ParserResult<Node> LeftParen(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.LParen);
        private static ParserResult<Node> RightParen(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.RParen);
        private static ParserResult<Node> PowerSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Power);
        private static ParserResult<Node> MultiplySymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Star);
        private static ParserResult<Node> DivideSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Slash);
        private static ParserResult<Node> ModuloSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Percent);
        private static ParserResult<Node> AddSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Plus);
        private static ParserResult<Node> SubtractSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Dash);
        private static ParserResult<Node> GreaterThanSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.RAngle);
        private static ParserResult<Node> GreaterThanEqualSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.GEQ);
        private static ParserResult<Node> LessThanSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.LAngle);
        private static ParserResult<Node> LessThanEqualSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.LEQ);
        private static ParserResult<Node> EqualToSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.EQU);
        private static ParserResult<Node> NotEqualToSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.NEQ);
        private static ParserResult<Node> IfThenSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Quest);
        private static ParserResult<Node> ElseSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Colon);
        private static ParserResult<Node> AndSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.AND);
        private static ParserResult<Node> OrSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.OR);

        private static ParserResult<Node> MultiplicativeOperator(ParserInputs<CG5Token> input)
        {
            return input.FirstOf(
                MultiplySymbol,
                DivideSymbol,
                ModuloSymbol);
        }

        private static ParserResult<Node> AdditiveOperator(ParserInputs<CG5Token> input)
        {
            return input.FirstOf(AddSymbol, SubtractSymbol);
        }

        private static ParserResult<Node> RelationalOperator(ParserInputs<CG5Token> input)
        {
            return input.FirstOf(
                GreaterThanSymbol,
                GreaterThanEqualSymbol,
                LessThanSymbol,
                LessThanEqualSymbol);
        }

        private static ParserResult<Node> EqualityOperator(ParserInputs<CG5Token> input)
        {
            return input.FirstOf(
                EqualToSymbol,
                NotEqualToSymbol);
        }

        private static ParserResult<Node> MatchConstant(ParserInputs<CG5Token> input, CG5Token kind, Func<ReadOnlyMemory<char>, Node> nodeFunc)
        {
            var tokens = input.Source.Span;
            if (tokens.Length == 0) return default;
            if (tokens[0].Kind != kind) return default;
            return new ParserResult<Node>(1, nodeFunc(tokens[0].Source));
        }

        private static ParserResult<Node> NullConstant(ParserInputs<CG5Token> input) => MatchConstant(input, CG5Token.Null, NullConstantNode.Create);
        private static ParserResult<Node> BooleanConstant(ParserInputs<CG5Token> input) => MatchConstant(input, CG5Token.Bool, BooleanConstantNode.Create);
        private static ParserResult<Node> StringConstant(ParserInputs<CG5Token> input) => MatchConstant(input, CG5Token.Str, StringConstantNode.Create);
        private static ParserResult<Node> NumberConstant(ParserInputs<CG5Token> input) => MatchConstant(input, CG5Token.Num, NumericConstantNode.Create);
        private static ParserResult<Node> AnyConstant(ParserInputs<CG5Token> input) => input.FirstOf(NullConstant, BooleanConstant, StringConstant, NumberConstant);

        private static Node BinaryExpressionBuilder(Node left, Node symbol, Node right)
        {
            return new BinaryExpressionNode() { Left = left, Op = symbol.ToBinaryOperator(), Right = right };
        }

        // -------------------- precedence 0 ----------------------------------------
        private static ParserResult<Node> GroupExpression(ParserInputs<CG5Token> input) => Combinators.AllOf(input, LeftParen, AnyExpression, RightParen, (left, expr, right) => expr);
        private static ParserResult<Node> Precedence0(ParserInputs<CG5Token> input) => input.FirstOf(VariableName, AnyConstant, GroupExpression);

        // -------------------- precedence 1 ----------------------------------------
        private static ParserResult<Node> PowerExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence0, PowerSymbol, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence1(ParserInputs<CG5Token> input) => input.FirstOf(PowerExpression, Precedence0);

        // -------------------- precedence 2 ----------------------------------------
        private static ParserResult<Node> MultiplyExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence1, MultiplicativeOperator, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence2(ParserInputs<CG5Token> input) => input.FirstOf(MultiplyExpression, Precedence1);

        // -------------------- precedence 3 ----------------------------------------
        private static ParserResult<Node> AdditiveExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence2, AdditiveOperator, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence3(ParserInputs<CG5Token> input) => input.FirstOf(AdditiveExpression, Precedence2);

        // -------------------- precedence 4 ----------------------------------------
        private static ParserResult<Node> RelationalExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence3, RelationalOperator, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence4(ParserInputs<CG5Token> input) => input.FirstOf(RelationalExpression, Precedence3);

        // -------------------- precedence 5 ----------------------------------------
        private static ParserResult<Node> EqualityExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence4, EqualityOperator, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence5(ParserInputs<CG5Token> input) => input.FirstOf(EqualityExpression, Precedence4);

        // -------------------- precedence 6 ----------------------------------------
        private static ParserResult<Node> AndExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence5, AndSymbol, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence6(ParserInputs<CG5Token> input) => input.FirstOf(AndExpression, Precedence5);

        // -------------------- precedence 7 ----------------------------------------
        private static ParserResult<Node> OrExpression(ParserInputs<CG5Token> input) => Combinators.Chain(input, Precedence6, OrSymbol, BinaryExpressionBuilder);
        private static ParserResult<Node> Precedence7(ParserInputs<CG5Token> input) => input.FirstOf(OrExpression, Precedence6);

        // -------------------- precedence 8 ----------------------------------------
        private static ParserResult<Node> IfThenElseExpression(ParserInputs<CG5Token> input)
        {
            return Combinators.AllOf(input, Precedence7, IfThenSymbol, Precedence7, ElseSymbol, Precedence7,
                (ifExpr, notUsed1, thenExpr, notUsed2, elseExpr) => TertiaryExpressionNode.Create(TertiaryOperator.IfThenElse, ifExpr, thenExpr, elseExpr));
        }

        private static ParserResult<Node> Precedence8(ParserInputs<CG5Token> input) => input.FirstOf(IfThenElseExpression, Precedence7);

        // -------------------- precedence 9 ----------------------------------------
        private static ParserResult<Node> AssignSymbol(ParserInputs<CG5Token> input) => MatchSymbol(input, CG5Token.Assign);
        private static ParserResult<Node> VariableName(ParserInputs<CG5Token> input)
        {
            var tokens = input.Source.Span;
            if (tokens.Length == 0) return default;
            if (tokens[0].Kind != CG5Token.Var) return default;
            return new ParserResult<Node>(1, VariableNode.Create(new string(tokens[0].Source.Span)));
        }

        private static ParserResult<Node> AssignExpression(ParserInputs<CG5Token> input)
        {
            return Combinators.AllOf(input, VariableName, AssignSymbol, Precedence8,
                (varName, symbol, expr) => BinaryExpressionNode.Create(BinaryOperator.Assign, varName, expr));
        }

        private static ParserResult<Node> Precedence9(ParserInputs<CG5Token> input) => input.FirstOf(AssignExpression, Precedence8);

        // -------------------- least precedent ---------------------------------------

        private static ParserResult<Node> AnyExpression(ParserInputs<CG5Token> input) => Precedence9(input);

        // -------------------- end of grammar ----------------------------------------

        public CG5Parser() : base(new CG5Lexer()) { }
        protected override IEnumerable<Token<CG5Token>> OnFilterTokens(IEnumerable<Token<CG5Token>> tokens) => tokens.SelectCodeTokens();
        protected override Node OnMakeErrorNode(string message) => new ErrorNode() { Message = message };
        protected override bool OnTryMatch(ReadOnlyMemory<Token<CG5Token>> tokens, out int consumed, out Node? result)
        {
            ParserInputs<CG5Token> inputs = new ParserInputs<CG5Token>(tokens);
            var parsed = AnyExpression(inputs);
            consumed = parsed.Consumed;
            result = parsed.Result;
            return parsed.Matched;
        }
    }
}
