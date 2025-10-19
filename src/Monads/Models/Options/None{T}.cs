namespace Monads.Models.Options;

/// <summary>
/// Represents an option with no value.
/// </summary>
public sealed record None<T>() : Option<T>
    where T : notnull
{
    /// <inheritdoc/>
    public override bool HasValue => false;
}
