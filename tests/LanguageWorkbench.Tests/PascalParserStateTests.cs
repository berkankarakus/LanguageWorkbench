using FrontEnd.Input;
using FrontEnd.Pascal.Parsing.TopDown;
using FrontEnd.Pascal.Scanning;
using FrontEnd.Pascal.Types.Factory;
using FrontEnd.Pascal.Types.Implementations;
using FrontEnd.Pascal.Types.Interfaces;
using Intermediate.Symbols.Implementations;
using Intermediate.Symbols.Interfaces;

namespace LanguageWorkbench.Tests;

[TestClass]
public sealed class PascalParserStateTests
{
    [TestMethod]
    public void ParseWhenParsersAreSeparateKeepsSymbolTablesAndErrorCountsIndependent()
    {
        PascalParserTopDown invalidParser = CreateAndParse("BEGIN first := 1");
        PascalParserTopDown validParser = CreateAndParse("BEGIN second := 2 END.");

        Assert.IsTrue(invalidParser.ErrorCount > 0);
        Assert.AreEqual(0, validParser.ErrorCount);
        Assert.IsNotNull(LookupProgramLocal(invalidParser, "first"));
        Assert.IsNotNull(LookupProgramLocal(validParser, "second"));
        Assert.IsNull(LookupProgramLocal(validParser, "first"));
    }

    [TestMethod]
    public void InitializeWhenPredefinedInstancesAreSeparateKeepsTypeSpecsIndependent()
    {
        Predefined firstPredefined = new();
        firstPredefined.Initialize(new SymbolTableStack());

        Predefined secondPredefined = new();
        secondPredefined.Initialize(new SymbolTableStack());

        ITypeSpec stringType = PascalTypeFactory.CreateStringType("text", firstPredefined);

        Assert.AreNotSame(firstPredefined.IntegerType, secondPredefined.IntegerType);
        Assert.AreNotSame(firstPredefined.CharType, secondPredefined.CharType);
        Assert.IsTrue(stringType.IsPascalString());
    }

    private static PascalParserTopDown CreateAndParse(string program)
    {
        using SourceReader source = new(new StringReader(program));
        PascalParserTopDown parser = new(new PascalScanner(source));
        parser.Parse();
        return parser;
    }

    private static ISymbolTableEntry? LookupProgramLocal(PascalParserTopDown parser, string name)
    {
        ISymbolTableEntry programId = parser.SymbolTableStack.ProgramId
            ?? throw new AssertFailedException("The parser did not create a program identifier.");

        ISymbolTable symbolTable = programId[SymbolTableKey.RoutineSymbolTable] as ISymbolTable
            ?? throw new AssertFailedException("The program identifier has no routine symbol table.");

        return symbolTable.Lookup(name);
    }
}
