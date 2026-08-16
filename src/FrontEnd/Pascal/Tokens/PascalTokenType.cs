using FrontEnd.Tokens;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Represents a Pascal-specific token type.
/// </summary>
public sealed class PascalTokenType : ITokenType
{
    private PascalTokenType(string text) => Text = text;

    /// <summary>
    /// Gets the source text associated with this token type.
    /// </summary>
    public string Text { get; }

    // Reserved words
    public static PascalTokenType And { get; } = new("and");
    public static PascalTokenType Array { get; } = new("array");
    public static PascalTokenType Begin { get; } = new("begin");
    public static PascalTokenType Case { get; } = new("case");
    public static PascalTokenType Const { get; } = new("const");
    public static PascalTokenType Div { get; } = new("div");
    public static PascalTokenType Do { get; } = new("do");
    public static PascalTokenType DownTo { get; } = new("downto");
    public static PascalTokenType Else { get; } = new("else");
    public static PascalTokenType End { get; } = new("end");
    public static PascalTokenType File { get; } = new("file");
    public static PascalTokenType For { get; } = new("for");
    public static PascalTokenType Function { get; } = new("function");
    public static PascalTokenType GoTo { get; } = new("goto");
    public static PascalTokenType If { get; } = new("if");
    public static PascalTokenType In { get; } = new("in");
    public static PascalTokenType Label { get; } = new("label");
    public static PascalTokenType Mod { get; } = new("mod");
    public static PascalTokenType Nil { get; } = new("nil");
    public static PascalTokenType Not { get; } = new("not");
    public static PascalTokenType Of { get; } = new("of");
    public static PascalTokenType Or { get; } = new("or");
    public static PascalTokenType Packed { get; } = new("packed");
    public static PascalTokenType Procedure { get; } = new("procedure");
    public static PascalTokenType Program { get; } = new("program");
    public static PascalTokenType Record { get; } = new("record");
    public static PascalTokenType Repeat { get; } = new("repeat");
    public static PascalTokenType Set { get; } = new("set");
    public static PascalTokenType Then { get; } = new("then");
    public static PascalTokenType To { get; } = new("to");
    public static PascalTokenType Type { get; } = new("type");
    public static PascalTokenType Until { get; } = new("until");
    public static PascalTokenType Var { get; } = new("var");
    public static PascalTokenType While { get; } = new("while");
    public static PascalTokenType With { get; } = new("with");

    // Special symbols
    public static PascalTokenType Plus { get; } = new("+");
    public static PascalTokenType Minus { get; } = new("-");
    public static PascalTokenType Star { get; } = new("*");
    public static PascalTokenType Slash { get; } = new("/");
    public static PascalTokenType ColonEquals { get; } = new(":=");
    public static PascalTokenType Dot { get; } = new(".");
    public static PascalTokenType Comma { get; } = new(",");
    public static PascalTokenType SemiColon { get; } = new(";");
    public static PascalTokenType Colon { get; } = new(":");
    public static PascalTokenType Quote { get; } = new("'");
    public new static PascalTokenType Equals { get; } = new("=");
    public static PascalTokenType NotEquals { get; } = new("<>");
    public static PascalTokenType LessThan { get; } = new("<");
    public static PascalTokenType LessEquals { get; } = new("<=");
    public static PascalTokenType GreaterEquals { get; } = new(">=");
    public static PascalTokenType GreaterThan { get; } = new(">");
    public static PascalTokenType LeftParen { get; } = new("(");
    public static PascalTokenType RightParen { get; } = new(")");
    public static PascalTokenType LeftBracket { get; } = new("[");
    public static PascalTokenType RightBracket { get; } = new("]");
    public static PascalTokenType LeftBrace { get; } = new("{");
    public static PascalTokenType RightBrace { get; } = new("}");
    public static PascalTokenType UpArrow { get; } = new("^");
    public static PascalTokenType DotDot { get; } = new("..");

    public static PascalTokenType Identifier { get; } = new("identifier");
    public static PascalTokenType Integer { get; } = new("integer");
    public static PascalTokenType Real { get; } = new("real");
    public static PascalTokenType String { get; } = new("string");
    public static PascalTokenType Error { get; } = new("error");
    public static PascalTokenType EndOfFile { get; } = new("end of file");

    /// <summary>
    /// Lowercase Pascal reserved words, indexed by their source text.
    /// </summary>
    public static IReadOnlySet<string> ReservedWords { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        And.Text,
        Array.Text,
        Begin.Text,
        Case.Text,
        Const.Text,
        Div.Text,
        Do.Text,
        DownTo.Text,
        Else.Text,
        End.Text,
        File.Text,
        For.Text,
        Function.Text,
        GoTo.Text,
        If.Text,
        In.Text,
        Label.Text,
        Mod.Text,
        Nil.Text,
        Not.Text,
        Of.Text,
        Or.Text,
        Packed.Text,
        Procedure.Text,
        Program.Text,
        Record.Text,
        Repeat.Text,
        Set.Text,
        Then.Text,
        To.Text,
        Type.Text,
        Until.Text,
        Var.Text,
        While.Text,
        With.Text
    };

    /// <summary>
    /// Pascal special symbols indexed by their source text.
    /// </summary>
    public static Dictionary<string, PascalTokenType> SpecialSymbols { get; } = new(StringComparer.Ordinal)
    {
        [Plus.Text] = Plus,
        [Minus.Text] = Minus,
        [Star.Text] = Star,
        [Slash.Text] = Slash,
        [ColonEquals.Text] = ColonEquals,
        [Dot.Text] = Dot,
        [Comma.Text] = Comma,
        [SemiColon.Text] = SemiColon,
        [Colon.Text] = Colon,
        [Quote.Text] = Quote,
        [Equals.Text] = Equals,
        [NotEquals.Text] = NotEquals,
        [LessThan.Text] = LessThan,
        [LessEquals.Text] = LessEquals,
        [GreaterEquals.Text] = GreaterEquals,
        [GreaterThan.Text] = GreaterThan,
        [LeftParen.Text] = LeftParen,
        [RightParen.Text] = RightParen,
        [LeftBracket.Text] = LeftBracket,
        [RightBracket.Text] = RightBracket,
        [LeftBrace.Text] = LeftBrace,
        [RightBrace.Text] = RightBrace,
        [UpArrow.Text] = UpArrow,
        [DotDot.Text] = DotDot
    };

    public override string ToString() => Text;
}
