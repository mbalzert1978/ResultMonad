// <copyright file="MapOrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapOrElseExtension"/> type.
/// </summary>
public sealed class MapOrElseTests
{
    private const int TestValue = 42;
    private const int FallbackValue = -1;

    [Fact]
    public void MapOrElse_WhenSelfIsSome_ShouldReturnMappedValueWithoutInvokingFallback()
    {
        var self = Option.Some(TestValue);
        bool fallbackInvoked = false;

        int result = self.MapOrElse(
            () =>
            {
                fallbackInvoked = true;
                return FallbackValue;
            },
            value => value * 2
        );

        result.Should().Be(84);
        fallbackInvoked.Should().BeFalse();
    }

    [Fact]
    public void MapOrElse_WhenSelfIsNone_ShouldReturnFallbackResultWithoutInvokingOperation()
    {
        var self = Option.None<int>();
        bool operationInvoked = false;

        int result = self.MapOrElse(
            () => FallbackValue,
            value =>
            {
                operationInvoked = true;
                return value * 2;
            }
        );

        result.Should().Be(FallbackValue);
        operationInvoked.Should().BeFalse();
    }

    [Fact]
    public void MapOrElse_WhenFallbackIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<int> act = () => self.MapOrElse<int, int>(null!, value => value);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapOrElse_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<int> act = () => self.MapOrElse<int, int>(() => FallbackValue, null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
