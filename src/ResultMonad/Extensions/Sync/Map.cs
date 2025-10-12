using static ResultMonad.ResultFactory;

namespace ResultMonad.Extensions.Sync;

public static class MapExtension
{
    /// <summary>
    /// Transforms the success value of the result using the provided mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="E">The type of the error value.</typeparam>
    /// <typeparam name="U">The type of the new success value after transformation.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="operation">The mapping function to apply to the success value.</param>
    /// <returns>A new result with the transformed success value or the original error.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="operation"/> is <c>null</c>.</exception>
    public static Result<U, E> Map<T, E, U>(this Result<T, E> self, Func<T, U> operation)
        where T : notnull
        where E : notnull
        where U : notnull
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(operation);

        return self.Match(value => Success<U, E>(operation(value)), Failure<U, E>);
    }
}
