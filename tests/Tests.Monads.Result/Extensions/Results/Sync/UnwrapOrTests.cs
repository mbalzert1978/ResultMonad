// <copyright file="UnwrapOrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrExtension"/> type.
/// </summary>
public sealed class UnwrapOrTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int Fallback = -1;

    [Fact]
    public void UnwrapOr_WhenOk_ShouldReturnInnerValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int unwrapped = result.UnwrapOr(Fallback);

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public void UnwrapOr_WhenErr_ShouldReturnFallback()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int unwrapped = result.UnwrapOr(Fallback);

        unwrapped.Should().Be(Fallback);
    }

    [Fact]
    public void UnwrapOr_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<int> act = () => result.UnwrapOr(Fallback);

        act.Should().Throw<ArgumentNullException>();
    }
}
