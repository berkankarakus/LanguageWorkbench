using System.Text;
using Intermediate.Code.Implementations;
using Intermediate.Code.Interfaces;
using Intermediate.Symbols.Implementations;

namespace Intermediate.Code.Utilities;

/// <summary>
/// Writes a human-readable representation of an intermediate-code tree.
/// </summary>
public class ParseTreePrinter(TextWriter textWriter)
{
    private const int IndentWidth = 4;
    private const int LineWidth = 80;

    private int _length;
    private readonly string _indent = new(c: ' ', count: IndentWidth); // indent spaces
    private string _indentation = ""; // indentation of a line
    private readonly StringBuilder _line = new();

    /// <summary>
    /// Writes intermediate code as a parse tree.
    /// </summary>
    /// <param name="intermediateCode">The intermediate code to write.</param>
    public void Print(IIntermediateCode intermediateCode)
    {
        textWriter.WriteLine("\n===== INTERMEDIATE CODE =====");

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
        Append(text: $"<{intermediateCode}");

        PrintAttributes(intermediateCode);
        PrintTypeSpec(intermediateCode);

        List<IIntermediateCodeNode>? childNodes = intermediateCode?.Children;

        // Print the node's children followed by the closing tag.
        if (childNodes is { Count: > 0 })
        {
            Append(text: ">");
            PrintLine();

            PrintChildNodes(childNodes);
            Append(text: _indentation);
            Append(text: $"</{intermediateCode}>");
        }

        else
        {
            Append(text: " ");
            Append(text: "/>");
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

        string text = $"{keyString?.ToLowerInvariant()}=\"{valueString}\"";
        Append(text: " ");
        Append(text);

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
    private void Append(string text)
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

        _line.Append(text);
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
}
