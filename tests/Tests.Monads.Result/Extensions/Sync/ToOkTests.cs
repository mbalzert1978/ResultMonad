// <copyright file="ToOkTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="ToOkExtension"/> type.
/// </summary>
public sealed class ToOkTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public void ToOk_WhenOk_ShouldReturnSomeWithValue()
    {
        Result<int, string> result = Ok<int, string>(SuccessValue);

        Option<int> option = result.ToOk();

        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public void ToOk_WhenErr_ShouldReturnNone()
    {
        Result<int, string> result = Err<int, string>(ErrorMessage);

        Option<int> option = result.ToOk();

        option.IsNone.Should().BeTrue();
    }

    [Fact]
    public void ToOk_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Result<int, string> result = null!;

        Func<Option<int>> act = () => result.ToOk();

        act.Should().Throw<ArgumentNullException>();
    }
}
