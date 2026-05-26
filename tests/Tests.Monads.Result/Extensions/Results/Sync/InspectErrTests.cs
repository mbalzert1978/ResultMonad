// <copyright file="InspectErrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="InspectErrExtension"/> type.
/// </summary>
public sealed class InspectErrTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void InspectErr_WhenErr_ShouldInvokeActionWithError()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);
        string seen = string.Empty;

        result.InspectErr(err => seen = err);

        seen.Should().Be(ErrorMessage);
    }

    [Fact]
    public void InspectErr_WhenErr_ShouldReturnOriginalResult()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Result<int, string> returned = result.InspectErr(_ => { });

        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void InspectErr_WhenOk_ShouldNotInvokeAction()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        bool invoked = false;

        result.InspectErr(_ => invoked = true);

        invoked.Should().BeFalse();
    }

    [Fact]
    public void InspectErr_WhenOk_ShouldReturnOriginalResult()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Result<int, string> returned = result.InspectErr(_ => { });

        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void InspectErr_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Result<int, string>> act = () => result.InspectErr(_ => { });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void InspectErr_WhenActionIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Func<Result<int, string>> act = () => result.InspectErr(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
