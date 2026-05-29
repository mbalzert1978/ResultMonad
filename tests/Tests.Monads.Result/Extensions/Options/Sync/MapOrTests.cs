// <copyright file="MapOrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapOrExtension"/> type.
/// </summary>
public sealed class MapOrTests
{
    private const int TestValue = 42;
    private const int Fallback = -1;

    [Fact]
    public void MapOr_WhenSelfIsSome_ShouldReturnMappedValue()
    {
        var self = Option.Some(TestValue);

        int result = self.MapOr(Fallback, value => value * 2);

        result.Should().Be(84);
    }

    [Fact]
    public void MapOr_WhenSelfIsNone_ShouldReturnFallbackWithoutInvokingOperation()
    {
        var self = Option.None<int>();
        bool invoked = false;

        int result = self.MapOr(
            Fallback,
            value =>
            {
                invoked = true;
                return value * 2;
            }
        );

        result.Should().Be(Fallback);
        invoked.Should().BeFalse();
    }

    [Fact]
    public void MapOr_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<int> act = () => self.MapOr<int, int>(Fallback, null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
