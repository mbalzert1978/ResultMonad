// <copyright file="XorTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="XorExtension"/> type.
/// </summary>
public sealed class XorTests
{
    [Fact]
    public void Xor_WhenSelfIsSomeAndOtherIsNone_ShouldReturnSelf()
    {
        var self = Option.Some(1);
        var other = Option.None<int>();

        Option<int> result = self.Xor(other);

        result.Should().Be(self);
    }

    [Fact]
    public void Xor_WhenSelfIsNoneAndOtherIsSome_ShouldReturnOther()
    {
        var self = Option.None<int>();
        var other = Option.Some(2);

        Option<int> result = self.Xor(other);

        result.Should().Be(other);
    }

    [Fact]
    public void Xor_WhenBothAreSome_ShouldReturnNone()
    {
        var self = Option.Some(1);
        var other = Option.Some(2);

        Option<int> result = self.Xor(other);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Xor_WhenBothAreNone_ShouldReturnNone()
    {
        var self = Option.None<int>();
        var other = Option.None<int>();

        Option<int> result = self.Xor(other);

        result.IsNone.Should().BeTrue();
    }
}
