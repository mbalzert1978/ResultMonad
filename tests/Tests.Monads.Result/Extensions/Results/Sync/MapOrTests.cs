// <copyright file="MapOrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="MapOrExtension"/> type.
/// </summary>
public sealed class MapOrTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int Fallback = -1;

    [Fact]
    public void MapOr_WhenOk_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int mapped = result.MapOr(Fallback, value => value * 2);

        mapped.Should().Be(84);
    }

    [Fact]
    public void MapOr_WhenErr_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int mapped = result.MapOr(Fallback, value => value * 2);

        mapped.Should().Be(Fallback);
    }

    [Fact]
    public void MapOr_WhenErr_ShouldNotInvokeOperation()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);
        bool invoked = false;

        result.MapOr(Fallback, value =>
        {
            invoked = true;
            return value * 2;
        });

        invoked.Should().BeFalse();
    }

    [Fact]
    public void MapOr_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<int> act = () => result.MapOr(Fallback, value => value * 2);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapOr_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<int> act = () => result.MapOr<int, string, int>(Fallback, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapOr_WhenMappingToDifferentType_ShouldReturnMappedValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        string mapped = result.MapOr("fallback", value => $"value:{value}");

        mapped.Should().Be("value:42");
    }
}
