// <copyright file="Map.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for transforming values within <see cref="Option{T}"/> instances.
/// </summary>
public static class MapExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Transforms the wrapped value using the specified mapping function when this option is Some.
        /// </summary>
        /// <typeparam name="U">The type of the transformed value.</typeparam>
        /// <param name="operation">The function to apply to the wrapped value.</param>
        /// <returns>
        /// A new <see cref="Option{U}"/> containing the mapped value when this option is Some;
        /// otherwise <see cref="Option.None{U}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public Option<U> Map<U>(Func<T, U> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(value => Option.Some(operation(value)), Option.None<U>);
        }
    }
}
