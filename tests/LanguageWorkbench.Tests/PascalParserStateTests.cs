using FrontEnd.Input;
using FrontEnd.Pascal.Parsing.TopDown;
using FrontEnd.Pascal.Scanning;
using FrontEnd.Pascal.Symbols.Implementations;
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

    [TestMethod]
    public void ParseWhenConstantDeclarationIsPresentStoresItsValueAndType()
    {
        PascalParserTopDown parser = CreateAndParse(
            "CONST answer = 42; BEGIN value := answer END."
        );

        Assert.AreEqual(0, parser.ErrorCount);

        ISymbolTableEntry constantId = LookupProgramLocal(parser, "answer")
            ?? throw new AssertFailedException("The constant was not entered into the symbol table.");

        Assert.AreEqual(Definition.Constant, constantId[PascalSymbolTableKey.Definition]);
        Assert.AreEqual(42, constantId[SymbolTableKey.ConstantValue]);
        Assert.IsInstanceOfType<ITypeSpec>(constantId[PascalSymbolTableKey.TypeSpec]);
    }

    [TestMethod]
    public void ParseWhenSubrangeTypeUsesChildParserSharesTranslationContext()
    {
        PascalParserTopDown parser = CreateAndParse(
            "TYPE digit = 0..9; BEGIN value := 1 END."
        );

        Assert.AreEqual(0, parser.ErrorCount);

        ISymbolTableEntry typeId = LookupProgramLocal(parser, "digit")
            ?? throw new AssertFailedException("The type was not entered into the symbol table.");

        ITypeSpec typeSpec = typeId[PascalSymbolTableKey.TypeSpec] as ITypeSpec
            ?? throw new AssertFailedException("The type specification was not stored.");

        Assert.AreEqual(TypeForm.Subrange, typeSpec.Form);
        Assert.AreEqual(0, typeSpec[TypeKey.SubrangeMinValue]);
        Assert.AreEqual(9, typeSpec[TypeKey.SubrangeMaxValue]);
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
