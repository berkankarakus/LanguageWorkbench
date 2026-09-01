using FrontEnd.Pascal.ErrorHandling;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Factory;
using Intermediate.Symbols.Interfaces;

namespace FrontEnd.Pascal.Parsing;

/// <summary>
/// Parses Pascal expressions into intermediate-code nodes.
/// </summary>
/// <param name="parent">The parser that provides the scanner and parser state.</param>
public class ExpressionParser(PascalParserTopDown parent) : StatementParser(parent)
{
    // Set of additive operators.
    private static readonly HashSet<ITokenType> AddOps =
    [
        PascalTokenType.Plus,
        PascalTokenType.Minus,
        PascalTokenType.Or
    ];

    // Maps Pascal additive operators to their intermediate-code node types.
    private static readonly Dictionary<ITokenType, IntermediateCodeNodeType> AdditiveOperatorNodeTypes = new()
    {
        [PascalTokenType.Plus] = IntermediateCodeNodeType.Add,
        [PascalTokenType.Minus] = IntermediateCodeNodeType.Subtract,
        [PascalTokenType.Or] = IntermediateCodeNodeType.Or
    };

    // Set of multiplicative operators.
    private static readonly HashSet<ITokenType> MultOps =
    [
        PascalTokenType.Star,
        PascalTokenType.Slash,
        PascalTokenType.Div,
        PascalTokenType.Mod,
        PascalTokenType.And
    ];

    // Map multiplicative operator tokens to node types.
    private static readonly Dictionary<ITokenType, IntermediateCodeNodeType> MultiplicativeOperatorNodeTypes = new()
    {
        [PascalTokenType.Star] = IntermediateCodeNodeType.Multiply,
        [PascalTokenType.Slash] = IntermediateCodeNodeType.FloatDivide,
        [PascalTokenType.Div] = IntermediateCodeNodeType.IntegerDivide,
        [PascalTokenType.Mod] = IntermediateCodeNodeType.Mod,
        [PascalTokenType.And] = IntermediateCodeNodeType.And
    };

    /// <summary>
    /// Contains the Pascal operators that compare two expressions.
    /// </summary>
    private static readonly HashSet<PascalTokenType> RelOps =
    [
        PascalTokenType.Equals,
        PascalTokenType.NotEquals,
        PascalTokenType.LessThan,
        PascalTokenType.LessEquals,
        PascalTokenType.GreaterThan,
        PascalTokenType.GreaterEquals
    ];

    /// <summary>
    /// Maps Pascal relational operators to their intermediate-code node types.
    /// </summary>
    private static readonly Dictionary<ITokenType, IntermediateCodeNodeType> RelationalOperatorNodeTypes = new()
    {
        [PascalTokenType.Equals] = IntermediateCodeNodeType.Equal,
        [PascalTokenType.NotEquals] = IntermediateCodeNodeType.NotEqual,
        [PascalTokenType.LessThan] = IntermediateCodeNodeType.LessThan,
        [PascalTokenType.LessEquals] = IntermediateCodeNodeType.LessThanOrEqual,
        [PascalTokenType.GreaterThan] = IntermediateCodeNodeType.GreaterThan,
        [PascalTokenType.GreaterEquals] = IntermediateCodeNodeType.GreaterThanOrEqual
    };

    /// <summary>
    /// Parses an expression beginning with the specified token.
    /// </summary>
    /// <param name="token">The first token of the expression.</param>
    /// <returns>The intermediate-code node for the expression.</returns>
    public override IIntermediateCodeNode? Parse(Token token)
    {
        return ParseExpression(token);
    }

    private IIntermediateCodeNode? ParseExpression(Token? token)
    {
        // The first simple expression forms the initial expression tree.
        IIntermediateCodeNode? rootNode = ParseSimpleExpression(token);
        token = CurrentToken();

        if (token?.Type is null)
            return rootNode;

        ITokenType tokenType = token.Type;

        // A simple expression is complete when no relational operator follows it.
        if (!RelOps.Contains(tokenType))
            return rootNode;

        // Make the relational operator the new root and attach the left operand.
        IntermediateCodeNodeType codeNodeType = RelationalOperatorNodeTypes[tokenType];
        IIntermediateCodeNode opNode = IntermediateCodeFactory.CreateIntermediateCodeNode(codeNodeType);
        opNode.AddChild(rootNode);

        // Consume the operator, then parse and attach its right operand.
        token = NextToken();
        opNode.AddChild(ParseSimpleExpression(token));
        rootNode = opNode;

        return rootNode;
    }

    /// <summary>
    /// Parses a simple expression.
    /// </summary>
    /// <returns>The intermediate-code node for the simple expression.</returns>
    private IIntermediateCodeNode? ParseSimpleExpression(Token? token)
    {
        ITokenType? signType = null;

        // Look for a leading + or - sign.
        ITokenType? tokenType = token?.Type;

        if (tokenType == PascalTokenType.Plus || tokenType == PascalTokenType.Minus)
        {
            signType = tokenType;
            token = NextToken();
        }

        // Parse a term & make the root of its tree the root node.
        IIntermediateCodeNode? rootNode = ParseTerm(token);

        // Was there a leading - sign?
        if (signType == PascalTokenType.Minus)
        {
            // Create a NEGATE node & adopt the current tree as its child. The NEGATE node becomes the new root node.
            var negateNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Negate);
            negateNode.AddChild(rootNode);
            rootNode = negateNode;
        }

        token = CurrentToken();
        tokenType = token?.Type;

        // Loop over additive operators.
        while (tokenType is not null && AddOps.Contains(tokenType))
        {
            // Create a new operator node & adopt the current tree as its first child.
            IntermediateCodeNodeType codeNodeType = AdditiveOperatorNodeTypes[tokenType];
            IIntermediateCodeNode opNode = IntermediateCodeFactory.CreateIntermediateCodeNode(codeNodeType);
            opNode.AddChild(rootNode);

            // consume the operator.
            token = NextToken();

            // Parse another term. The operator node adopts the term's tree as its second child.
            opNode.AddChild(ParseTerm(token));

            // The operator node becomes the new root node.
            rootNode = opNode;

            token = CurrentToken();
            tokenType = token?.Type;
        }

        return rootNode;
    }

    /// <summary>
    /// Parses a term within a simple expression.
    /// </summary>
    /// <returns>The intermediate-code node for the term.</returns>
    public IIntermediateCodeNode? ParseTerm(Token? token)
    {
        // Parse a factor & make its node the root node.
        IIntermediateCodeNode? codeNode = ParseFactor(token);
        IIntermediateCodeNode? rootNode = codeNode;

        // Get the operator.
        token = CurrentToken();
        ITokenType? tokenType = token?.Type;

        while (tokenType is not null && MultOps.Contains(tokenType))
        {
            // Create a new operator node & adopt the current tree as its first child.
            IIntermediateCodeNodeType codeNodeType = MultiplicativeOperatorNodeTypes[tokenType];
            IIntermediateCodeNode opNode = IntermediateCodeFactory.CreateIntermediateCodeNode(codeNodeType);
            opNode.AddChild(rootNode);

            // Consume the operator.
            token = NextToken();

            // Parse another factor. The operator node adopts the term's tree as its second child.
            opNode.AddChild(ParseFactor(token));

            // The operator node becomes the new root node.
            rootNode = opNode;

            token = CurrentToken();
            tokenType = token?.Type;
        }

        return rootNode;
    }

    /// <summary>
    /// Parses a factor within a term.
    /// </summary>
    /// <returns>The intermediate-code node for the factor.</returns>
    public IIntermediateCodeNode? ParseFactor(Token? token)
    {
        ITokenType? tokenType = token?.Type;
        IIntermediateCodeNode? rootNode = null;

        if (token?.Text is null || tokenType is null)
            return rootNode;

        if (tokenType == PascalTokenType.Identifier)
        {
            // Look up the identifier in the symbol table stack.
            // Flag the identifier as undefined if it's not found.
            string name = token.Text.ToLowerInvariant();
            ISymbolTableEntry? entry = SymbolTableStack.Lookup(name);

            if (entry is null)
            {
                ErrorHandler.Flag(token, PascalErrorCode.IdentifierUndefined, parser: this);
                entry = SymbolTableStack.EnterLocal(name);
            }

            rootNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Variable);
            rootNode[IntermediateCodeKey.Id] = entry;
            entry.AppendLineNumber(token.LineNumber);

            // Consume the identifier.
            _ = NextToken();
        }

        else if (tokenType == PascalTokenType.Integer)
        {
            rootNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.IntegerConstant);
            rootNode[IntermediateCodeKey.Value] = token.Value;

            // Consume the number.
            _ = NextToken();
        }

        else if (tokenType == PascalTokenType.Real)
        {
            rootNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.RealConstant);
            rootNode[IntermediateCodeKey.SourceText] = token.Text;
            rootNode[IntermediateCodeKey.Value] = token.Value;

            // Consume the number.
            _ = NextToken();
        }

        else if (tokenType == PascalTokenType.String)
        {
            string? value = (string?)token.Value;

            rootNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.StringConstant);
            rootNode[IntermediateCodeKey.Value] = value;

            // Consume the string.
            _ = NextToken();
        }

        else if (tokenType == PascalTokenType.Not)
        {
            // Consume the NOT.
            _ = NextToken();

            rootNode = IntermediateCodeFactory.CreateIntermediateCodeNode(IntermediateCodeNodeType.Not);

            // Parse the factor. The NOT node adopts the factor node as its child.
            rootNode.AddChild(ParseFactor(token));
        }

        else if (tokenType == PascalTokenType.LeftParen)
        {
            // Consume the (.
            token = NextToken();

            rootNode = ParseExpression(token);

            // Look for mathing ).
            token = CurrentToken();

            if (token is not null)
            {
                if (token.Type == PascalTokenType.RightParen)
                {
                    // Consume the ).
                    _ = NextToken();
                }

                else
                    ErrorHandler.Flag(token, PascalErrorCode.MissingRightParen, parser: this);
            }
        }

        else
            ErrorHandler.Flag(token, PascalErrorCode.UnexpectedToken, parser: this);

        return rootNode;
    }
}
