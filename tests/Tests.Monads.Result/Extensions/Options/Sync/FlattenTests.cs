// <copyright file="FlattenTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="FlattenExtension"/> type.
/// </summary>
public sealed class FlattenTests
{
    private const int TestValue = 42;

    [Fact]
    public void Flatten_WhenSelfIsSomeOfSome_ShouldReturnInnerSome()
    {
        var self = Option.Some(Option.Some(TestValue));

        Option<int> result = self.Flatten();

        result.Should().Be(Option.Some(TestValue));
    }

    [Fact]
    public void Flatten_WhenSelfIsSomeOfNone_ShouldReturnNone()
    {
        var self = Option.Some(Option.None<int>());

        Option<int> result = self.Flatten();

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Flatten_WhenSelfIsNone_ShouldReturnNone()
    {
        var self = Option.None<Option<int>>();

        Option<int> result = self.Flatten();

        result.IsNone.Should().BeTrue();
    }
}
