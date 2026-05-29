// <copyright file="OrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="OrExtension"/> type.
/// </summary>
public sealed class OrTests
{
    private const int TestValue = 42;
    private const int FallbackValue = 99;

    [Fact]
    public void Or_WhenSelfIsSome_ShouldReturnSelf()
    {
        var self = Option.Some(TestValue);
        var other = Option.Some(FallbackValue);

        Option<int> result = self.Or(other);

        result.Should().Be(self);
    }

    [Fact]
    public void Or_WhenSelfIsNone_ShouldReturnOther()
    {
        var self = Option.None<int>();
        var other = Option.Some(FallbackValue);

        Option<int> result = self.Or(other);

        result.Should().Be(other);
    }

    [Fact]
    public void Or_WhenBothAreNone_ShouldReturnNone()
    {
        var self = Option.None<int>();
        var other = Option.None<int>();

        Option<int> result = self.Or(other);

        result.IsNone.Should().BeTrue();
    }
}
