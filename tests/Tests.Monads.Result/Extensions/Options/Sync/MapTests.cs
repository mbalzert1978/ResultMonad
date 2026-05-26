// <copyright file="MapTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapExtension"/> type.
/// </summary>
public sealed class MapTests
{
    private const int TestValue = 42;

    [Fact]
    public void Map_WhenSelfIsSome_ShouldReturnSomeOfMappedValue()
    {
        var self = Option.Some(TestValue);

        Option<int> result = self.Map(value => value * 2);

        result.Should().Be(Option.Some(84));
    }

    [Fact]
    public void Map_WhenSelfIsNone_ShouldReturnNoneWithoutInvokingOperation()
    {
        var self = Option.None<int>();
        bool invoked = false;

        Option<int> result = self.Map(value =>
        {
            invoked = true;
            return value * 2;
        });

        result.IsNone.Should().BeTrue();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void Map_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Option<int>> act = () => self.Map<int, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
