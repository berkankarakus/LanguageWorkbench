using FrontEnd.Input;
using FrontEnd.Pascal.Scanning;
using FrontEnd.Pascal.Tokens;
using FrontEnd.Tokens;

namespace LanguageWorkbench.Tests;

[TestClass]
public sealed class PascalScannerTests
{
    [TestMethod]
    public void NextTokenWhenSourceIsRepresentativePascalExtractsExpectedTypesAndValues()
    {
        using SourceReader source = new(new StringReader("BEGIN answer := 42; text := 'ok' END."));
        PascalScanner scanner = new(source);

        Token begin = scanner.NextToken();
        Token identifier = scanner.NextToken();
        Token colonEquals = scanner.NextToken();
        Token integer = scanner.NextToken();
        Token semicolon = scanner.NextToken();
        Token stringIdentifier = scanner.NextToken();
        Token secondColonEquals = scanner.NextToken();
        Token stringToken = scanner.NextToken();
        Token end = scanner.NextToken();
        Token period = scanner.NextToken();

        Assert.AreSame(PascalTokenType.Begin, begin.Type);
        Assert.AreSame(PascalTokenType.Identifier, identifier.Type);
        Assert.AreEqual("answer", identifier.Text);
        Assert.AreSame(PascalTokenType.ColonEquals, colonEquals.Type);
        Assert.AreSame(PascalTokenType.Integer, integer.Type);
        Assert.AreEqual(42, integer.Value);
        Assert.AreSame(PascalTokenType.SemiColon, semicolon.Type);
        Assert.AreSame(PascalTokenType.Identifier, stringIdentifier.Type);
        Assert.AreEqual("text", stringIdentifier.Text);
        Assert.AreSame(PascalTokenType.ColonEquals, secondColonEquals.Type);
        Assert.AreSame(PascalTokenType.String, stringToken.Type);
        Assert.AreEqual("ok", stringToken.Value);
        Assert.AreSame(PascalTokenType.End, end.Type);
        Assert.AreSame(PascalTokenType.Dot, period.Type);
    }
}
