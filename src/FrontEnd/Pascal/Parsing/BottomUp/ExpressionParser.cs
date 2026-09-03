using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing.BottomUp;

/// <summary>
/// Reduces Pascal expressions with operand and operator stacks.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public sealed class ExpressionParser(PascalParser parent) : PascalParser(parent)
{
    /// <summary>
    /// Parses an expression and leaves its terminating token as the current token.
    /// </summary>
    /// <param name="token">The first token of the expression.</param>
    /// <returns>The reduced intermediate-code node.</returns>
    public IIntermediateCodeNode Parse(ref Token token)
    {
        Stack<IIntermediateCodeNode> operands = new();
        Stack<(ITokenType Type, bool IsUnary)> operators = new();
        bool expectingOperand = true;

        while (CanContinueExpression(token))
        {
            ITokenType? tokenType = token.Type;

            if (tokenType is null)
            {
                ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);
                continue;
            }

            if (IsOperand(tokenType))
            {
                if (!expectingOperand)
                    ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);

                operands.Push(CreateOperandNode(token));
                token = NextToken();
                expectingOperand = false;
            }

            else if (tokenType == PascalTokenType.LeftParen)
            {
                if (!expectingOperand)
                    ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);

                operators.Push((tokenType, IsUnary: false));
                token = NextToken();
            }

            else if (tokenType == PascalTokenType.RightParen)
            {
                if (expectingOperand)
                    ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);

                ReduceUntilLeftParenthesis(operands, operators, token);
                token = NextToken();
                expectingOperand = false;
            }

            else if (IsOperator(tokenType))
            {
                if (expectingOperand)
                {
                    if (tokenType == PascalTokenType.Plus)
                    {
                        token = NextToken();
                        continue;
                    }

                    if (IsInvalidUnaryOperator(tokenType))
                        ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);

                    operators.Push((tokenType, IsUnary: true));
                    token = NextToken();
                    continue;
                }

                ReduceHigherPrecedenceOperators(operands, operators, tokenType);
                operators.Push((tokenType, IsUnary: false));
                token = NextToken();
                expectingOperand = true;
            }

            else
                ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);
        }

        if (expectingOperand)
            ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);

        while (operators.Count > 0)
        {
            if (operators.Peek().Type == PascalTokenType.LeftParen)
                ErrorHandler.Flag(token, PascalErrorCode.MissingRightParen, parser: this);

            ReduceOperator(operands, operation: operators.Pop(), token);
        }

        if (operands.Count != 1)
            ErrorHandler.Flag(token, PascalErrorCode.InvalidExpression, parser: this);

        return operands.Pop();
    }

    private static bool CanContinueExpression(Token token)
    {
        return
            token is not EofToken &&
            !IsExpressionTerminator(token.Type);
    }

    private static bool IsExpressionTerminator(ITokenType? tokenType)
    {
        return
            tokenType == PascalTokenType.SemiColon ||
            tokenType == PascalTokenType.End ||
            tokenType == PascalTokenType.Dot;
    }

    private static bool IsOperand(ITokenType tokenType)
    {
        return
            tokenType == PascalTokenType.Identifier ||
            tokenType == PascalTokenType.Integer ||
            tokenType == PascalTokenType.Real ||
            tokenType == PascalTokenType.String;
    }

    private static bool IsOperator(ITokenType tokenType)
    {
        return
            GetPrecedence(tokenType) >= 0 ||
            tokenType == PascalTokenType.Not;
    }

    private static bool IsInvalidUnaryOperator(ITokenType tokenType)
    {
        return
            tokenType != PascalTokenType.Minus &&
            tokenType != PascalTokenType.Not;
    }

    private static int GetPrecedence(ITokenType tokenType)
    {
        if (IsRelationalOperator(tokenType))
            return 1;

        if (IsAdditiveOperator(tokenType))
            return 2;

        return IsMultiplicativeOperator(tokenType)
            ? 3
            : -1;
    }

    private static bool IsRelationalOperator(ITokenType tokenType)
    {
        return
            tokenType == PascalTokenType.Equals ||
            tokenType == PascalTokenType.NotEquals ||
            tokenType == PascalTokenType.LessThan ||
            tokenType == PascalTokenType.LessEquals ||
            tokenType == PascalTokenType.GreaterThan ||
            tokenType == PascalTokenType.GreaterEquals;
    }

    private static bool IsAdditiveOperator(ITokenType tokenType)
    {
        return
            tokenType == PascalTokenType.Plus ||
            tokenType == PascalTokenType.Minus ||
            tokenType == PascalTokenType.Or;
    }

    private static bool IsMultiplicativeOperator(ITokenType tokenType)
    {
        return
            tokenType == PascalTokenType.Star ||
            tokenType == PascalTokenType.Slash ||
            tokenType == PascalTokenType.Div ||
            tokenType == PascalTokenType.Mod ||
            tokenType == PascalTokenType.And;
    }

    private void ReduceHigherPrecedenceOperators(
        Stack<IIntermediateCodeNode> operands,
        Stack<(ITokenType Type, bool IsUnary)> operators,
        ITokenType tokenType
    )
    {
        while (HasHigherOrEqualPrecedenceOperator(operators, tokenType))
            ReduceOperator(operands, operation: operators.Pop(), token: null);
    }

    private static bool HasHigherOrEqualPrecedenceOperator(
        Stack<(ITokenType Type, bool IsUnary)> operators,
        ITokenType tokenType
    )
    {
        return
            operators.Count > 0 &&
            operators.Peek().Type != PascalTokenType.LeftParen &&
            (
                operators.Peek().IsUnary ||
                GetPrecedence(operators.Peek().Type) >= GetPrecedence(tokenType)
            );
    }

    private void ReduceUntilLeftParenthesis(
        Stack<IIntermediateCodeNode> operands,
        Stack<(ITokenType Type, bool IsUnary)> operators,
        Token token
    )
    {
        while (HasUnreducedOperator(operators))
            ReduceOperator(operands, operation: operators.Pop(), token);

        if (operators.Count == 0)
            ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);

        _ = operators.Pop();
    }

    private static bool HasUnreducedOperator(Stack<(ITokenType Type, bool IsUnary)> operators)
    {
        return
            operators.Count > 0 &&
            operators.Peek().Type != PascalTokenType.LeftParen;
    }

    private void ReduceOperator(
        Stack<IIntermediateCodeNode> operands,
        (ITokenType Type, bool IsUnary) operation,
        Token? token
    )
    {
        if (operation.IsUnary)
        {
            if (operands.Count < 1)
                ErrorHandler.Flag(token!, PascalErrorCode.InvalidExpression, parser: this);

            var unaryOperatorNode = IntermediateCodeFactory.CreateIntermediateCodeNode(
                operation.Type == PascalTokenType.Minus
                    ? IntermediateCodeNodeType.Negate
                    : IntermediateCodeNodeType.Not
            );

            unaryOperatorNode.AddChild(operands.Pop());
            operands.Push(unaryOperatorNode);
            return;
        }

        if (operands.Count < 2)
            ErrorHandler.Flag(token!, PascalErrorCode.InvalidExpression, parser: this);

        IIntermediateCodeNode rightOperand = operands.Pop();
        IIntermediateCodeNode leftOperand = operands.Pop();
        var operatorNodeType = GetOperatorNodeType(operation.Type);
        var binaryOperatorNode = IntermediateCodeFactory.CreateIntermediateCodeNode(operatorNodeType);
        binaryOperatorNode.AddChild(leftOperand);
        binaryOperatorNode.AddChild(rightOperand);
        operands.Push(binaryOperatorNode);
    }

    private static IntermediateCodeNodeType GetOperatorNodeType(ITokenType tokenType)
    {
        return tokenType switch
        {
            _ when tokenType == PascalTokenType.Equals => IntermediateCodeNodeType.Equal,
            _ when tokenType == PascalTokenType.NotEquals => IntermediateCodeNodeType.NotEqual,
            _ when tokenType == PascalTokenType.LessThan => IntermediateCodeNodeType.LessThan,
            _ when tokenType == PascalTokenType.LessEquals => IntermediateCodeNodeType.LessThanOrEqual,
            _ when tokenType == PascalTokenType.GreaterThan => IntermediateCodeNodeType.GreaterThan,
            _ when tokenType == PascalTokenType.GreaterEquals => IntermediateCodeNodeType.GreaterThanOrEqual,
            _ when tokenType == PascalTokenType.Plus => IntermediateCodeNodeType.Add,
            _ when tokenType == PascalTokenType.Minus => IntermediateCodeNodeType.Subtract,
            _ when tokenType == PascalTokenType.Or => IntermediateCodeNodeType.Or,
            _ when tokenType == PascalTokenType.Star => IntermediateCodeNodeType.Multiply,
            _ when tokenType == PascalTokenType.Slash => IntermediateCodeNodeType.FloatDivide,
            _ when tokenType == PascalTokenType.Div => IntermediateCodeNodeType.IntegerDivide,
            _ when tokenType == PascalTokenType.Mod => IntermediateCodeNodeType.Mod,
            _ when tokenType == PascalTokenType.And => IntermediateCodeNodeType.And,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType))
        };
    }

    private IIntermediateCodeNode CreateOperandNode(Token token)
    {
        ArgumentNullException.ThrowIfNull(token.Type);

        if (token.Type == PascalTokenType.Identifier)
        {
            ArgumentNullException.ThrowIfNull(token.Text);
            string name = token.Text.ToLowerInvariant();
            ISymbolTableEntry? entry = SymbolTableStack.Lookup(name);

            if (entry is null)
            {
                ErrorHandler.Flag(token, PascalErrorCode.IdentifierUndefined, parser: this);
                entry = SymbolTableStack.EnterLocal(name);
            }

            entry.AppendLineNumber(token.LineNumber);
            var variableNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Variable);
            variableNode[IntermediateCodeKey.Id] = entry;
            return variableNode;
        }

        if (token.Type == PascalTokenType.Integer)
            return CreateConstantNode(IntermediateCodeNodeType.IntegerConstant, token.Value);

        if (token.Type == PascalTokenType.Real)
        {
            IIntermediateCodeNode realNode = CreateConstantNode(IntermediateCodeNodeType.RealConstant, token.Value);
            realNode[IntermediateCodeKey.SourceText] = token.Text;
            return realNode;
        }

        return CreateConstantNode(IntermediateCodeNodeType.StringConstant, token.Value);
    }

    private static IIntermediateCodeNode CreateConstantNode(IIntermediateCodeNodeType nodeType, object? value)
    {
        IIntermediateCodeNode node = IntermediateCodeFactory.CreateIntermediateCodeNode(nodeType);
        node[IntermediateCodeKey.Value] = value;
        return node;
    }
}
