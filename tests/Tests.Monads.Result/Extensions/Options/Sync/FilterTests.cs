// <copyright file="FilterTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="FilterExtension"/> type.
/// </summary>
public sealed class FilterTests
{
    private const int TestValue = 42;

    [Fact]
    public void Filter_WhenSelfIsSomeAndPredicateMatches_ShouldReturnSelf()
    {
        var self = Option.Some(TestValue);

        Option<int> result = self.Filter(value => value > 0);

        result.Should().Be(self);
    }

    [Fact]
    public void Filter_WhenSelfIsSomeAndPredicateDoesNotMatch_ShouldReturnNone()
    {
        var self = Option.Some(TestValue);

        Option<int> result = self.Filter(value => value < 0);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Filter_WhenSelfIsNone_ShouldReturnNoneWithoutInvokingPredicate()
    {
        var self = Option.None<int>();
        bool invoked = false;

        Option<int> result = self.Filter(_ => { invoked = true; return true; });

        result.IsNone.Should().BeTrue();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void Filter_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Option<int>> act = () => self.Filter(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
