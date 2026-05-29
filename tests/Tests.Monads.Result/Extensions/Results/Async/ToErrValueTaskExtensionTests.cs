// <copyright file="ToErrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="ToErrValueTaskExtension"/> type.
/// </summary>
public sealed class ToErrValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task ToErrAsync_WhenValueTaskErr_ShouldReturnSome()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Err<int, string>(ErrorMessage)
        );

        Option<string> option = await resultTask.ToErrAsync();

        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task ToErrAsync_WhenValueTaskOk_ShouldReturnNone()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Ok<int, string>(SuccessValue)
        );

        Option<string> option = await resultTask.ToErrAsync();

        option.IsNone.Should().BeTrue();
    }
}
