// <copyright file="Bind.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Sync;

/// <summary>
/// Provides extension members for chaining option-returning operations on <see cref="Option{T}"/> instances.
/// </summary>
public static class BindExtension
{
    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Invokes <paramref name="operation"/> with the wrapped value when this option is Some,
        /// returning the produced option; otherwise returns <see cref="Option.None{U}"/>.
        /// </summary>
        /// <typeparam name="U">The type of the value in the resulting option.</typeparam>
        /// <param name="operation">The option-returning function to invoke when this option is Some.</param>
        /// <returns>
        /// The option produced by <paramref name="operation"/> when this option is Some;
        /// otherwise <see cref="Option.None{U}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public Option<U> Bind<U>(Func<T, Option<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return self.Match(operation, Option.None<U>);
        }
    }
}
