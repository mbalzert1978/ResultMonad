// <copyright file="UnwrapOrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrExtension"/> type.
/// </summary>
public sealed class UnwrapOrTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public void UnwrapOr_WhenSelfIsSome_ShouldReturnWrappedValue()
    {
        var self = Option.Some(TestValue);

        int result = self.UnwrapOr(FallbackValue);

        result.Should().Be(TestValue);
    }

    [Fact]
    public void UnwrapOr_WhenSelfIsNone_ShouldReturnFallback()
    {
        var self = Option.None<int>();

        int result = self.UnwrapOr(FallbackValue);

        result.Should().Be(FallbackValue);
    }
}
