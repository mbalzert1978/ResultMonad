// <copyright file="ZipTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="ZipExtension"/> type.
/// </summary>
public sealed class ZipTests
{
    [Fact]
    public void Zip_WhenBothAreSome_ShouldReturnSomeOfTuple()
    {
        var self = Option.Some(1);
        var other = Option.Some("two");

        Option<(int, string)> result = self.Zip(other);

        result.Should().Be(Option.Some((1, "two")));
    }

    [Fact]
    public void Zip_WhenSelfIsSomeAndOtherIsNone_ShouldReturnNone()
    {
        var self = Option.Some(1);
        var other = Option.None<string>();

        Option<(int, string)> result = self.Zip(other);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Zip_WhenSelfIsNone_ShouldReturnNone()
    {
        var self = Option.None<int>();
        var other = Option.Some("two");

        Option<(int, string)> result = self.Zip(other);

        result.IsNone.Should().BeTrue();
    }
}
