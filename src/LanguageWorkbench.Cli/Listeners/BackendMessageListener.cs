using Messaging;
using System.Globalization;

namespace LanguageWorkbench.Cli.Listeners;

/// <summary>
/// Renders messages produced by a compiler or interpreter back end.
/// </summary>
/// <param name="useColor">Whether terminal colors should be used for output.</param>
public class BackendMessageListener(bool useColor = false) : IMessageListener
{
    private const string OutputPrecisionEnvironmentVariable = "LANGUAGE_WORKBENCH_OUTPUT_PRECISION";
    private readonly bool _useColor = useColor;
    private bool _isFirstOutputMessage = true;

    /// <summary>
    /// Renders a back-end message when its payload has the expected shape.
    /// </summary>
    /// <param name="message">The back-end message to render.</param>
    public void MessageReceived(Message message)
    {
        switch (message.Type)
        {
            case MessageType.InterpreterSummary
                when message.Body
                    is object?[]
                        and [int executionCount, int runtimeErrorCount, float elapsedTime]:
                PrintInterpreterSummary(executionCount, runtimeErrorCount, elapsedTime);
                break;

            case MessageType.CompilerSummary
                when message.Body is object?[] and [int instructionCount, float elapsedTime]:
                Console.WriteLine(
                    $"\n{instructionCount,10} instructions generated.\n{elapsedTime,10:F2} seconds total code generation time."
                );
                break;

            case MessageType.Assign
                when message.Body is object?[] and [int lineNumber, string variableName, var value]:
                PrintAssignment(lineNumber, variableName, value);
                break;

            case MessageType.RuntimeError
                when message.Body is object?[] and [string errorMessage, int lineNumber]:
                Console.WriteLine($"*** RUNTIME ERROR AT LINE {lineNumber:000}: {errorMessage}");
                break;
        }
    }

    private void PrintAssignment(int lineNumber, string variableName, object? value)
    {
        if (_isFirstOutputMessage)
        {
            Console.WriteLine();
            WriteColored("===== OUTPUT =====", ConsoleColor.Cyan);
            Console.WriteLine();
            _isFirstOutputMessage = false;
        }

        WriteColored("LINE ", ConsoleColor.Cyan);
        WriteColored($"{lineNumber:000}", ConsoleColor.Magenta);
        Console.Write(": ");
        WriteColored(variableName, ConsoleColor.Yellow);
        Console.Write(" = ");
        WriteColored(FormatValue(value), GetValueColor(value));
        Console.WriteLine();
    }

    private void PrintInterpreterSummary(int executionCount, int runtimeErrorCount, float elapsedTime)
    {
        Console.WriteLine();
        WriteColored($"{executionCount,10}", ConsoleColor.Magenta);
        WriteColored(" statements executed.", ConsoleColor.Cyan);
        Console.WriteLine();
        WriteColored($"{runtimeErrorCount,10}", ConsoleColor.Magenta);
        WriteColored(" runtime errors.", ConsoleColor.Cyan);
        Console.WriteLine();
        WriteColored($"{elapsedTime,10:F2}", ConsoleColor.Magenta);
        WriteColored(" seconds total execution time.", ConsoleColor.Cyan);
        Console.WriteLine();
    }

    private static string FormatValue(object? value)
    {
        int? precision = GetOutputPrecision();

        return value switch
        {
            float floatValue => FormatFloatingPoint(floatValue, precision),
            double doubleValue => FormatFloatingPoint(doubleValue, precision),
            _ => value?.ToString() ?? "null"
        };
    }

    private static string FormatFloatingPoint(float value, int? precision) => value.ToString(
        precision is int valuePrecision ? $"G{valuePrecision}" : null,
        CultureInfo.CurrentCulture
    );

    private static string FormatFloatingPoint(double value, int? precision) => value.ToString(
        precision is int valuePrecision ? $"G{valuePrecision}" : null,
        CultureInfo.CurrentCulture
    );

    private static int? GetOutputPrecision()
    {
        string? value = Environment.GetEnvironmentVariable(OutputPrecisionEnvironmentVariable);

        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int precision)
            && precision is > 0 and <= 17
            ? precision
            : null;
    }

    private static ConsoleColor GetValueColor(object? value) => value switch
    {
        string or char => ConsoleColor.Green,
        _ => ConsoleColor.Magenta
    };

    private void WriteColored(string text, ConsoleColor color)
    {
        if (!_useColor)
        {
            Console.Write(text);
            return;
        }

        ConsoleColor previousColor = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
