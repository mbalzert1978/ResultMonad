// <copyright file="ToErrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="ToErrExtension"/> type.
/// </summary>
public sealed class ToErrTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void ToErr_WhenErr_ShouldReturnSomeWithError()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Option<string> option = result.ToErr();

        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(ErrorMessage);
    }

    [Fact]
    public void ToErr_WhenOk_ShouldReturnNone()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Option<string> option = result.ToErr();

        option.IsNone.Should().BeTrue();
    }

    [Fact]
    public void ToErr_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Option<string>> act = () => result.ToErr();

        act.Should().Throw<ArgumentNullException>();
    }
}
