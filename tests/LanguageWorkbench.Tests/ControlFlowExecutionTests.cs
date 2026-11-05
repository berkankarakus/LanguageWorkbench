using Backend.Interpreter;
using FrontEnd.Input;
using FrontEnd.Pascal.Parsing.TopDown;
using FrontEnd.Pascal.Scanning;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Interfaces;
using Messaging;

namespace LanguageWorkbench.Tests;

[TestClass]
public sealed class ControlFlowExecutionTests
{
    [TestMethod]
    public void ExecuteWhenCaseHasMultipleConstantsAssignsMatchingBranchValue()
    {
        const string program = """
            BEGIN
                selector := 2;
                CASE selector OF
                    1: result := 10;
                    2, 3: result := 20
                END
            END.
            """;

        TestMessageListener listener = Execute(program);

        AssertAssignment(listener, "result", 20);
    }

    [TestMethod]
    public void ExecuteWhenRepeatConditionBecomesTrueAssignsFinalLoopValue()
    {
        const string program = """
            BEGIN
                i := 0;
                REPEAT
                    i := i + 1
                UNTIL i = 3
            END.
            """;

        TestMessageListener listener = Execute(program);

        AssertAssignment(listener, "i", 3);
    }

    [TestMethod]
    public void ExecuteWhenExecutorsAreSeparateKeepsTheirMessageListenersIsolated()
    {
        const string program = "BEGIN value := 1 END.";

        (IIntermediateCode firstCode, ISymbolTableStack firstSymbols) = Parse(program);
        Executor firstExecutor = new();
        TestMessageListener firstListener = new();
        firstExecutor.AddMessageListener(firstListener);
        firstExecutor.Process(firstCode, firstSymbols);
        int firstMessageCount = firstListener.Messages.Count;

        (IIntermediateCode secondCode, ISymbolTableStack secondSymbols) = Parse(program);
        Executor secondExecutor = new();
        TestMessageListener secondListener = new();
        secondExecutor.AddMessageListener(secondListener);
        secondExecutor.Process(secondCode, secondSymbols);

        Assert.AreEqual(firstMessageCount, firstListener.Messages.Count);
        Assert.IsTrue(secondListener.Messages.Count > 0);
    }

    private static TestMessageListener Execute(string program)
    {
        (IIntermediateCode code, ISymbolTableStack symbolTableStack) = Parse(program);

        Executor executor = new();
        TestMessageListener listener = new();
        executor.AddMessageListener(listener);
        executor.Process(code, symbolTableStack);

        return listener;
    }

    private static (IIntermediateCode Code, ISymbolTableStack SymbolTableStack) Parse(string program)
    {
        using SourceReader source = new(new StringReader(program));
        PascalParserTopDown parser = new(new PascalScanner(source));
        parser.Parse();
        Assert.AreEqual(0, parser.ErrorCount);

        return (parser.Code, parser.SymbolTableStack);
    }

    private static void AssertAssignment(TestMessageListener listener, string name, object expectedValue)
    {
        Message? assignmentMessage = listener.Messages.LastOrDefault(message =>
            IsAssignmentFor(message, name)
        );

        object?[] assignmentBody = assignmentMessage?.Body as object?[]
            ?? throw new AssertFailedException($"No assignment message was emitted for '{name}'.");

        Assert.AreEqual(expectedValue, assignmentBody[2]);
    }

    private static bool IsAssignmentFor(Message message, string name)
    {
        if (message.Type != MessageType.Assign)
            return false;

        if (message.Body is not object?[] { Length: 3 } body)
            return false;

        return body[1] is string variableName && variableName == name;
    }
}
