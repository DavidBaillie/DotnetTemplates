using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ExampleTemplate.WebApp.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed partial class TextAttribute
    : ValidationAttribute
{
    /// <summary>
    /// Only allow valid ASCII characters
    /// </summary>
    public bool AsciiOnly { get; init; }

    /// <summary>
    /// Valid ASCII is between SPACE char and ~ char
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^[ -~\r\n\t]*$")]
    private partial Regex AsciiRegex();

    /// <summary>
    /// If line break chars should be allowed
    /// </summary>
    public bool AllowLineBreaks { get; init; }

    [GeneratedRegex(@"[\p{Zl}\p{Zp}\r\n]")]
    private partial Regex LineBreakReqex();

    /// <summary>
    /// 0x00 through 0x1F + 7F are ASCII control codes. 
    /// Exception is made here for CR, LF, Space, and HT.
    /// </summary>
    [GeneratedRegex(@"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]")]
    private partial Regex AsciiControlCodes();

    public override bool IsValid(object? value)
        => IsValid(value, new ValidationContext(value ?? new())) == ValidationResult.Success;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not string stringValue)
            throw new InvalidOperationException($"{nameof(TextAttribute)} can only be applied to {nameof(String)}, but was applied to {value.GetType().Name}");

        if (!AllowLineBreaks && LineBreakReqex().IsMatch(stringValue))
            return new ValidationResult("Must not contain new line characters (CR, LF, Unicode [Zp] and [Zl]).");

        if (AsciiOnly && !AsciiRegex().IsMatch(stringValue))
            return new ValidationResult("Must contain only printable ASCII characters plus CR, LF, Space, and HT.");

        // These are always banned because they shouldn't need to be used and can actually be VERY dangerous:
        // https://bugzilla.mozilla.org/show_bug.cgi?id=968576
        if (AsciiControlCodes().IsMatch(stringValue))
            return new ValidationResult("Cannot contain ASCII control codes except CR, LF, Space, and HT.");

        return ValidationResult.Success;
    }
}