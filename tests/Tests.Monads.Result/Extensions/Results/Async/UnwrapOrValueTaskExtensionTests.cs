// <copyright file="UnwrapOrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrValueTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrValueTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int Fallback = -1;

    [Fact]
    public async Task UnwrapOrAsync_WhenValueTaskOk_ShouldReturnInnerValue()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Ok<int, string>(SuccessValue)
        );

        int unwrapped = await resultTask.UnwrapOrAsync(Fallback);

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrAsync_WhenValueTaskErr_ShouldReturnFallback()
    {
        ValueTask<Result<int, string>> resultTask = ValueTask.FromResult(
            Err<int, string>(ErrorMessage)
        );

        int unwrapped = await resultTask.UnwrapOrAsync(Fallback);

        unwrapped.Should().Be(Fallback);
    }
}
