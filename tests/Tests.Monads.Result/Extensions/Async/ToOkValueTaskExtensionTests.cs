// <copyright file="ToOkValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="ToOkValueTaskExtension"/> type.
/// </summary>
public sealed class ToOkValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task ToOkAsync_WhenValueTaskOk_ShouldReturnSome()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Ok<int, string>(SuccessValue));

        Option<int> option = await resultTask.ToOkAsync();

        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task ToOkAsync_WhenValueTaskErr_ShouldReturnNone()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(Err<int, string>(ErrorMessage));

        Option<int> option = await resultTask.ToOkAsync();

        option.IsNone.Should().BeTrue();
    }
}
