// <copyright file="PredicatesTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="PredicateExtension"/> type.
/// </summary>
public sealed class PredicatesTests
{
    [Fact]
    public void IsSomeAnd_WhenSelfIsSomeAndPredicateMatches_ShouldReturnTrue()
    {
        var self = Option.Some(42);

        bool result = self.IsSomeAnd(value => value > 0);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsSomeAnd_WhenSelfIsSomeAndPredicateDoesNotMatch_ShouldReturnFalse()
    {
        var self = Option.Some(42);

        bool result = self.IsSomeAnd(value => value < 0);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsSomeAnd_WhenSelfIsNone_ShouldReturnFalseWithoutInvokingPredicate()
    {
        var self = Option.None<int>();
        bool invoked = false;

        bool result = self.IsSomeAnd(_ =>
        {
            invoked = true;
            return true;
        });

        result.Should().BeFalse();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void IsSomeAnd_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(42);

        Func<bool> act = () => self.IsSomeAnd(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsNoneOr_WhenSelfIsNone_ShouldReturnTrueWithoutInvokingPredicate()
    {
        var self = Option.None<int>();
        bool invoked = false;

        bool result = self.IsNoneOr(_ =>
        {
            invoked = true;
            return false;
        });

        result.Should().BeTrue();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void IsNoneOr_WhenSelfIsSomeAndPredicateMatches_ShouldReturnTrue()
    {
        var self = Option.Some(42);

        bool result = self.IsNoneOr(value => value > 0);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsNoneOr_WhenSelfIsSomeAndPredicateDoesNotMatch_ShouldReturnFalse()
    {
        var self = Option.Some(42);

        bool result = self.IsNoneOr(value => value < 0);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsNoneOr_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(42);

        Func<bool> act = () => self.IsNoneOr(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
