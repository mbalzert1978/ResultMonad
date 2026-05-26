// <copyright file="InspectTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="InspectExtension"/> type.
/// </summary>
public sealed class InspectTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void Inspect_WhenOk_ShouldInvokeActionWithValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        int seen = 0;

        result.Inspect(value => seen = value);

        seen.Should().Be(SuccessValue);
    }

    [Fact]
    public void Inspect_WhenOk_ShouldReturnOriginalResult()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Result<int, string> returned = result.Inspect(_ => { });

        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void Inspect_WhenErr_ShouldNotInvokeAction()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);
        bool invoked = false;

        result.Inspect(_ => invoked = true);

        invoked.Should().BeFalse();
    }

    [Fact]
    public void Inspect_WhenErr_ShouldReturnOriginalResult()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Result<int, string> returned = result.Inspect(_ => { });

        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void Inspect_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Result<int, string>> act = () => result.Inspect(_ => { });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Inspect_WhenActionIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Func<Result<int, string>> act = () => result.Inspect(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Inspect_WhenChained_ShouldInvokeBothInOrder()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);
        var seen = new List<int>();

        result.Inspect(v => seen.Add(v)).Inspect(v => seen.Add(v * 2));

        seen.Should().Equal(SuccessValue, SuccessValue * 2);
    }
}
