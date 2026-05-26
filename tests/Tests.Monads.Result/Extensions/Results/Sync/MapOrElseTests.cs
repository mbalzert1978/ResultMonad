// <copyright file="MapOrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapOrElseExtension"/> type.
/// </summary>
public sealed class MapOrElseTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void MapOrElse_WhenOk_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int mapped = result.MapOrElse(err => err.Length, value => value * 2);

        mapped.Should().Be(84);
    }

    [Fact]
    public void MapOrElse_WhenErr_ShouldReturnFallbackComputedFromError()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int mapped = result.MapOrElse(err => err.Length, value => value * 2);

        mapped.Should().Be(4);
    }

    [Fact]
    public void MapOrElse_WhenOk_ShouldNotInvokeFallback()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        bool invoked = false;

        result.MapOrElse(
            err =>
            {
                invoked = true;
                return err.Length;
            },
            value => value * 2);

        invoked.Should().BeFalse();
    }

    [Fact]
    public void MapOrElse_WhenErr_ShouldNotInvokeOperation()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);
        bool invoked = false;

        result.MapOrElse(
            err => err.Length,
            value =>
            {
                invoked = true;
                return value * 2;
            });

        invoked.Should().BeFalse();
    }

    [Fact]
    public void MapOrElse_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<int> act = () => result.MapOrElse(err => err.Length, value => value * 2);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapOrElse_WhenFallbackIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<int> act = () => result.MapOrElse(null!, value => value * 2);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapOrElse_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<int> act = () => result.MapOrElse(err => err.Length, null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
