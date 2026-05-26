// <copyright file="BindValueTaskExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Monads.Options.Extensions.Async;

/// <summary>
/// Provides asynchronous extension members for chaining option-returning operations on <see cref="Option{T}"/> instances using <see cref="ValueTask{TResult}"/>.
/// </summary>
public static class BindValueTaskExtension
{
    extension<T>(ValueTask<Option<T>> self)
        where T : notnull
    {
        /// <summary>
        /// Awaits the option value task and chains it with a synchronous option-returning operation.
        /// </summary>
        /// <typeparam name="U">The value type of the resulting option.</typeparam>
        /// <param name="operation">The synchronous function invoked when the awaited option is Some.</param>
        /// <returns>A value task producing the option returned by <paramref name="operation"/> when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<Option<U>> BindAsync<U>(Func<T, Option<U>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, Option.None<U>).ConfigureAwait(false);
        }

        /// <summary>
        /// Awaits the option value task and chains it with an asynchronous option-returning operation.
        /// </summary>
        /// <typeparam name="U">The value type of the resulting option.</typeparam>
        /// <param name="operation">The asynchronous function invoked when the awaited option is Some.</param>
        /// <returns>A value task producing the option returned by <paramref name="operation"/> when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<Option<U>> BindAsync<U>(Func<T, ValueTask<Option<U>>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, () => new ValueTask<Option<U>>(Option.None<U>()))
                .ConfigureAwait(false);
        }
    }

    extension<T>(Option<T> self)
        where T : notnull
    {
        /// <summary>
        /// Chains this synchronous option with an asynchronous option-returning operation.
        /// </summary>
        /// <typeparam name="U">The value type of the resulting option.</typeparam>
        /// <param name="operation">The asynchronous function invoked when this option is Some.</param>
        /// <returns>A value task producing the option returned by <paramref name="operation"/> when Some; otherwise None.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
        public async ValueTask<Option<U>> BindAsync<U>(Func<T, ValueTask<Option<U>>> operation)
            where U : notnull
        {
            ArgumentNullException.ThrowIfNull(operation);

            return await self.MatchAsync(operation, () => new ValueTask<Option<U>>(Option.None<U>()))
                .ConfigureAwait(false);
        }
    }
}
