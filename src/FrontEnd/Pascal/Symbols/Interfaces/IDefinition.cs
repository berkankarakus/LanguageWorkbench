namespace FrontEnd.Pascal.Symbols.Interfaces;

/// <summary>
/// Identifies how a Pascal symbol is declared or used.
/// </summary>
public interface IDefinition
{
    /// <summary>
    /// Gets a display-friendly description of the definition.
    /// </summary>
    string Text { get; }
}
