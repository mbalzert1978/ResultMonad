// <copyright file="OrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="OrElseExtension"/> type.
/// </summary>
public sealed class OrElseTests
{
    private const int TestValue = 42;
    private const int FallbackValue = 99;

    [Fact]
    public void OrElse_WhenSelfIsSome_ShouldReturnSelfWithoutInvokingOperation()
    {
        var self = Option.Some(TestValue);
        bool invoked = false;

        Option<int> result = self.OrElse(() =>
        {
            invoked = true;
            return Option.Some(FallbackValue);
        });

        result.Should().Be(self);
        invoked.Should().BeFalse();
    }

    [Fact]
    public void OrElse_WhenSelfIsNone_ShouldReturnOperationResult()
    {
        var self = Option.None<int>();
        var fallback = Option.Some(FallbackValue);

        Option<int> result = self.OrElse(() => fallback);

        result.Should().Be(fallback);
    }

    [Fact]
    public void OrElse_WhenSelfIsNoneAndOperationReturnsNone_ShouldReturnNone()
    {
        var self = Option.None<int>();

        Option<int> result = self.OrElse(Option.None<int>);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void OrElse_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Option<int>> act = () => self.OrElse(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
