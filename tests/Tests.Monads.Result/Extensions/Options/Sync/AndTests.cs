// <copyright file="AndTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="AndExtension"/> type.
/// </summary>
public sealed class AndTests
{
    [Fact]
    public void And_WhenSelfIsSome_ShouldReturnOther()
    {
        var self = Option.Some(1);
        var other = Option.Some("two");

        Option<string> result = self.And(other);

        result.Should().Be(other);
    }

    [Fact]
    public void And_WhenSelfIsSomeAndOtherIsNone_ShouldReturnNone()
    {
        var self = Option.Some(1);
        var other = Option.None<string>();

        Option<string> result = self.And(other);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void And_WhenSelfIsNone_ShouldReturnNone()
    {
        var self = Option.None<int>();
        var other = Option.Some("two");

        Option<string> result = self.And(other);

        result.IsNone.Should().BeTrue();
    }
}
