using System.Text;
using FrontEnd.Input;
using FrontEnd.Pascal.ErrorHandling;

namespace FrontEnd.Pascal.Tokens;

/// <summary>
/// Represents a Pascal integer or real numeric literal token.
/// </summary>
public class PascalNumberToken(SourceReader source) : PascalToken(source)
{
    /// <summary>
    /// Extracts a Pascal numeric literal from the source.
    /// </summary>
    protected override void Extract()
    {
        StringBuilder textBuffer = new();
        ExtractNumber(textBuffer);
        Text = textBuffer.ToString();
    }

    /// <summary>
    /// Extracts the components of a Pascal numeric literal.
    /// </summary>
    /// <param name="textBuffer">Receives the source text of the numeric literal.</param>
    private void ExtractNumber(StringBuilder textBuffer)
    {
        string? fractionalDigits = null; // digits after the decimal point
        string? exponentDigits = null; // exponent digits
        char exponentSign = '+'; // exponent sign '+' or '-'
        bool sawDotDot = false;  // true if saw ".." token

        Type = PascalTokenType.Integer; // assume INTEGER type for now

        // Extract the digits of the whole part of the number.
        var wholeDigits = UnsignedIntegerDigits(textBuffer); // digits before the decimal point

        if (Type == PascalTokenType.Error || wholeDigits is null)
            return;

        // Is there a .?
        // It could be a decimal point or the start of a ".." token
        char currentChar = CurrentChar();

        if (currentChar == '.')
        {
            if (PeekChar() == '.')
                sawDotDot = true;

            else
            {
                Type = PascalTokenType.Real; // decimal point, so token type is REAL
                textBuffer.Append(currentChar);
                _ = NextChar(); // consume decimal point

                // Collect the digits of the fraction part of the number.
                fractionalDigits = UnsignedIntegerDigits(textBuffer);

                if (Type == PascalTokenType.Error || fractionalDigits is null)
                    return;
            }
        }

        // Is there an exponent?
        // There cannot be an exponent if we saw a ".." token.
        currentChar = CurrentChar();

        if (!sawDotDot && currentChar is 'E' or 'e')
        {
            Type = PascalTokenType.Real; // exponent, so token type is REAL
            textBuffer.Append(currentChar);
            currentChar = NextChar();

            // Exponent sign?
            if (currentChar is '+' or '-')
            {
                textBuffer.Append(currentChar);
                exponentSign = currentChar;
                _ = NextChar();
            }

            // Extract the digits of the exponent.
            exponentDigits = UnsignedIntegerDigits(textBuffer);

            if (Type == PascalTokenType.Error || exponentDigits is null)
                return;
        }

        // Compute the value after all parts of the number have been extracted.
        if (Type == PascalTokenType.Integer)
        {
            int integerValue = ComputeIntegerValue(wholeDigits);

            if (Type != PascalTokenType.Error)
                Value = integerValue;
        }
        else if (Type == PascalTokenType.Real)
        {
            double floatValue = ComputeFloatValue(wholeDigits, fractionalDigits, exponentDigits, exponentSign);

            if (Type != PascalTokenType.Error)
                Value = floatValue;
        }
    }

    /// <summary>
    /// Computes the <see cref="double"/> value of a real number literal.
    /// </summary>
    /// <param name="wholeDigits">Digits before the decimal point.</param>
    /// <param name="fractionalDigits">Digits after the decimal point, if present.</param>
    /// <param name="exponentDigits">Exponent digits, if present.</param>
    /// <param name="exponentSign">The exponent sign.</param>
    /// <returns>The parsed real value, or zero when parsing sets an error token type.</returns>
    private double ComputeFloatValue(string wholeDigits, string? fractionalDigits, string? exponentDigits, char exponentSign)
    {
        double floatValue = 0.0;
        int exponentValue = ComputeIntegerValue(exponentDigits);
        string digits = wholeDigits; // whole & fraction digits

        // Negate the exponent if the exponent is '-'.
        if (exponentSign == '-')
            exponentValue *= -1;

        // If there are any fractional digits, adjust the exponent value & append the fraction digits.
        if (fractionalDigits != null)
        {
            exponentValue -= fractionalDigits.Length;
            digits += fractionalDigits;
        }

        // Loop over the digits to compute the float value.
        int index = 0;

        while(index < digits.Length)
            floatValue = 10 * floatValue + (digits[index++] - '0');

        if (exponentValue != 0)
            floatValue *= Math.Pow(10, exponentValue);

        double result = floatValue;

        if (!double.IsInfinity(result))
            return result;

        Type = PascalTokenType.Error;
        Value = PascalErrorCode.RangeReal;
        return 0.0f;
    }

    /// <summary>
    /// Computes the integer value represented by a sequence of digits.
    /// </summary>
    /// <param name="digits">The decimal digits to convert.</param>
    /// <returns>The parsed integer value, or zero when the input is null or out of range.</returns>
    private int ComputeIntegerValue(string? digits)
    {
        // Return 0 if no digits.
        if (digits is null)
            return 0;

        int integerValue = 0;
        int prevValue = 0; // overflow occured if prevValue > integerValue
        int index = 0;

        // Loop over the digits to compute integer value as long as there is no overflow.
        while (index < digits.Length && integerValue >= prevValue)
        {
            prevValue = integerValue;
            integerValue = 10 * integerValue + (digits[index++] - '0');
        }

        // No overflow: return the integer value
        if (integerValue >= prevValue)
            return integerValue;

        // Overflow: set the integer value out of range error
        Type = PascalTokenType.Error;
        Value = PascalErrorCode.RangeInteger;

        return 0;
    }

    /// <summary>
    /// Extracts the digits of an unsigned integer component.
    /// </summary>
    /// <param name="textBuffer">Receives the source text of the numeric literal.</param>
    /// <returns>The extracted digits, or <see langword="null"/> when no digit is present.</returns>
    private string? UnsignedIntegerDigits(StringBuilder textBuffer)
    {
        char currentChar = CurrentChar();

        // Must have at least one digit.
        if (!char.IsDigit(currentChar))
        {
            Type = PascalTokenType.Error;
            Value = PascalErrorCode.InvalidNumber;

            return null;
        }

        // Extract the digits.
        StringBuilder digits = new();

        while (char.IsDigit(currentChar))
        {
            textBuffer.Append(currentChar);
            digits.Append(currentChar);
            currentChar = NextChar();
        }

        return digits.ToString();
    }
}
