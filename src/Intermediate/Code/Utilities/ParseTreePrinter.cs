using System.Text;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Implementations;

namespace Intermediate.Code.Utilities;

/// <summary>
/// Writes a human-readable representation of an intermediate-code tree.
/// </summary>
public class ParseTreePrinter(TextWriter textWriter, bool useColor = false)
{
    private const int IndentWidth = 4;
    private const int LineWidth = 80;
    private const string ResetColor = "\u001b[0m";

    private int _length;
    private readonly bool _useColor = useColor;
    private readonly string _indent = new(c: ' ', count: IndentWidth); // indent spaces
    private string _indentation = ""; // indentation of a line
    private readonly StringBuilder _line = new();

    /// <summary>
    /// Writes intermediate code as a parse tree.
    /// </summary>
    /// <param name="intermediateCode">The intermediate code to write.</param>
    public void Print(IIntermediateCode intermediateCode)
    {
        WriteLine("\n===== INTERMEDIATE CODE =====", ConsoleColor.Yellow);

        PrintNode((IntermediateCodeNode?)intermediateCode.Root);
        PrintLine();
    }

    /// <summary>
    /// Writes a parse-tree node.
    /// </summary>
    /// <param name="intermediateCode">The node to write.</param>
    private void PrintNode(IntermediateCodeNode? intermediateCode)
    {
        // opening tag
        Append(_indentation);
        Append("<");
        Append(intermediateCode?.ToString() ?? "null", ConsoleColor.Cyan);

        PrintAttributes(intermediateCode);
        PrintTypeSpec(intermediateCode);

        List<IIntermediateCodeNode>? childNodes = intermediateCode?.Children;

        // Print the node's children followed by the closing tag.
        if (childNodes is { Count: > 0 })
        {
            Append(">");
            PrintLine();

            PrintChildNodes(childNodes);
            Append(text: _indentation);
            Append("</");
            Append(intermediateCode?.ToString() ?? "null", ConsoleColor.Cyan);
            Append(">");
        }

        else
        {
            Append(text: " ");
            Append("/>");
        }

        PrintLine();
    }

    /// <summary>
    /// Writes child nodes at the next indentation level.
    /// </summary>
    /// <param name="childNodes">The child nodes to write.</param>
    private void PrintChildNodes(List<IIntermediateCodeNode> childNodes)
    {
        string saveIndentation = _indentation;
        _indentation += _indent;

        foreach (IIntermediateCodeNode childNode in childNodes)
            PrintNode((IntermediateCodeNode?)childNode);

        _indentation = saveIndentation;
    }

    /// <summary>
    /// Writes type-specific information for a parse-tree node.
    /// </summary>
    /// <param name="intermediateCode">The node whose type information is written.</param>
    private static void PrintTypeSpec(IntermediateCodeNode? intermediateCode)
    {
        // Do nothing for now.
    }

    /// <summary>
    /// Writes the attributes of a parse-tree node.
    /// </summary>
    /// <param name="intermediateCode">The node whose attributes are written.</param>
    private void PrintAttributes(IntermediateCodeNode? intermediateCode)
    {
        string saveIndentation = _indentation;
        _indentation += _indent;

        Dictionary<IIntermediateCodeKey, object> attributes = intermediateCode?.Attributes ?? [];

        foreach (var attribute in attributes)
        {
            if (attribute.Key.Equals(IntermediateCodeKey.SourceText))
                continue;

            object value = HasSourceText(attribute.Key, attributes)
                ? attributes[IntermediateCodeKey.SourceText]
                : attribute.Value;

            PrintAttribute(attribute.Key.ToString(), value);
        }

        _indentation = saveIndentation;
    }

    private static bool HasSourceText(
        IIntermediateCodeKey key,
        Dictionary<IIntermediateCodeKey, object> attributes
    )
    {
        return key.Equals(IntermediateCodeKey.Value) && attributes.ContainsKey(IntermediateCodeKey.SourceText);
    }

    /// <summary>
    /// Writes a node attribute as <c>key="value"</c>.
    /// </summary>
    /// <param name="keyString">The attribute name.</param>
    /// <param name="attributeValue">The attribute value.</param>
    private void PrintAttribute(string? keyString, object attributeValue)
    {
        // If the value is a symbol table entry, use the identifier's name. Else just use the value string.
        bool isSymbolTableEntry = attributeValue is SymbolTableEntry;
        string? valueString = isSymbolTableEntry
            ? ((SymbolTableEntry)attributeValue).Name
            : attributeValue.ToString();

        Append(text: " ");
        Append(keyString?.ToLowerInvariant() ?? "", ConsoleColor.Yellow);
        Append("=\"");
        Append(valueString ?? "", GetValueColor(attributeValue));
        Append("\"");

        // Include identifier's nesting level.
        if (isSymbolTableEntry)
        {
            int level = ((SymbolTableEntry)attributeValue).SymbolTable.NestingLevel;
            PrintAttribute("LEVEL", level);
        }
    }

    /// <summary>
    /// Appends text to the current output line.
    /// </summary>
    /// <param name="text">The text to append.</param>
    private void Append(string text, ConsoleColor? color = null)
    {
        int textLength = text.Length;
        bool lineBreak = false;

        // Wrap lines that are too long.
        if (_length + textLength > LineWidth)
        {
            PrintLine();
            _line.Append(_indentation);
            _length = _indentation.Length;
            lineBreak = true;
        }

        if (lineBreak && text.Equals(" ", StringComparison.Ordinal))
            return;

        AppendText(text, color);
        _length += text.Length;
    }

    /// <summary>
    /// Print an output line.
    /// </summary>
    private void PrintLine()
    {
        if (_length <= 0)
            return;

        textWriter.WriteLine(_line.ToString());
        _line.Length = 0;
        _length = 0;
    }

    /// <summary>
    /// Writes a standalone line, optionally using an ANSI terminal color.
    /// </summary>
    private void WriteLine(string text, ConsoleColor? color = null)
    {
        if (color is { } terminalColor && _useColor)
            textWriter.Write(GetColorSequence(terminalColor));

        textWriter.Write(text);

        if (color is not null && _useColor)
            textWriter.Write(ResetColor);

        textWriter.WriteLine();
    }

    /// <summary>
    /// Appends text to the current line, applying an ANSI terminal color when enabled.
    /// </summary>
    private void AppendText(string text, ConsoleColor? color)
    {
        if (color is { } terminalColor && _useColor)
            _line.Append(GetColorSequence(terminalColor));

        _line.Append(text);

        if (color is not null && _useColor)
            _line.Append(ResetColor);
    }

    private static ConsoleColor GetValueColor(object attributeValue)
    {
        return attributeValue switch
        {
            SymbolTableEntry => ConsoleColor.Green,
            string => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };
    }

    private static string GetColorSequence(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Cyan => "\u001b[96m",
            ConsoleColor.Green => "\u001b[92m",
            ConsoleColor.Magenta => "\u001b[95m",
            ConsoleColor.White => "\u001b[97m",
            ConsoleColor.Yellow => "\u001b[93m",
            _ => ResetColor
        };
    }
}
