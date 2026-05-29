// <copyright file="UnwrapOrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrElseExtension"/> type.
/// </summary>
public sealed class UnwrapOrElseTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void UnwrapOrElse_WhenOk_ShouldReturnInnerValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        int unwrapped = result.UnwrapOrElse(err => err.Length);

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public void UnwrapOrElse_WhenErr_ShouldReturnFallbackComputedFromError()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        int unwrapped = result.UnwrapOrElse(err => err.Length);

        unwrapped.Should().Be(4);
    }

    [Fact]
    public void UnwrapOrElse_WhenOk_ShouldNotInvokeFallback()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        bool invoked = false;

        result.UnwrapOrElse(err =>
        {
            invoked = true;
            return err.Length;
        });

        invoked.Should().BeFalse();
    }

    [Fact]
    public void UnwrapOrElse_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<int> act = () => result.UnwrapOrElse(err => err.Length);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UnwrapOrElse_WhenFallbackIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<int> act = () => result.UnwrapOrElse(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
