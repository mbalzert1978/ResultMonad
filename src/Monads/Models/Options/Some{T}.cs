namespace Monads.Models.Options;

/// <summary>
/// Represents an option containing a value.
/// </summary>
public sealed record Some<T>(T Value) : Option<T>
    where T : notnull
{
    /// <summary>
    /// Gets the value contained in this option.
    /// </summary>
    /// <value>
    /// The value of type <typeparamref name="T"/>.
    /// </value>
    /// <exception cref="ArgumentNullException">Thrown during construction if the provided value is <c>null</c>.</exception>
    public T Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));

    /// <inheritdoc/>
    public override bool HasValue => true;
}
