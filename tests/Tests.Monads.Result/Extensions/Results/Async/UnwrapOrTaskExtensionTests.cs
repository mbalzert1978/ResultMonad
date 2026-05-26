// <copyright file="UnwrapOrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="UnwrapOrTaskExtension"/> type.
/// </summary>
public sealed class UnwrapOrTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "Test error";
    private const int Fallback = -1;

    [Fact]
    public async Task UnwrapOrAsync_WhenTaskOk_ShouldReturnInnerValue()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        int unwrapped = await resultTask.UnwrapOrAsync(Fallback);

        unwrapped.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task UnwrapOrAsync_WhenTaskErr_ShouldReturnFallback()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        int unwrapped = await resultTask.UnwrapOrAsync(Fallback);

        unwrapped.Should().Be(Fallback);
    }

    [Fact]
    public async Task UnwrapOrAsync_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = null!;

        Func<Task> act = async () => await resultTask.UnwrapOrAsync(Fallback);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
