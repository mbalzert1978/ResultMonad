// <copyright file="UnwrapOrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrElseExtension"/> type.
/// </summary>
public sealed class UnwrapOrElseTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public void UnwrapOrElse_WhenSelfIsSome_ShouldReturnWrappedValueWithoutInvokingFallback()
    {
        var self = Option.Some(TestValue);
        bool invoked = false;

        int result = self.UnwrapOrElse(() => { invoked = true; return FallbackValue; });

        result.Should().Be(TestValue);
        invoked.Should().BeFalse();
    }

    [Fact]
    public void UnwrapOrElse_WhenSelfIsNone_ShouldReturnFallbackResult()
    {
        var self = Option.None<int>();

        int result = self.UnwrapOrElse(() => FallbackValue);

        result.Should().Be(FallbackValue);
    }

    [Fact]
    public void UnwrapOrElse_WhenFallbackIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<int> act = () => self.UnwrapOrElse(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
