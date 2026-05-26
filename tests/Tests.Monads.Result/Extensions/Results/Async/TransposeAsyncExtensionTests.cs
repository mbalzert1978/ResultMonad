// <copyright file="TransposeAsyncExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Tests.Monads.Results.Extensions.Results.Async;

/// <summary>
/// Contains unit tests for the <see cref="TransposeAsyncExtension"/> type.
/// </summary>
public sealed class TransposeAsyncExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task TransposeAsync_WhenTaskOkSome_ShouldReturnSomeOk()
    {
        Task<Result<Option<int>, string>> task = Task.FromResult(
            Ok<Option<int>, string>(Option.Some(SuccessValue)));

        Option<Result<int, string>> transposed = await task.TransposeAsync();

        transposed.IsSome.Should().BeTrue();
    }

    [Fact]
    public async Task TransposeAsync_WhenTaskOkNone_ShouldReturnNone()
    {
        Task<Result<Option<int>, string>> task = Task.FromResult(
            Ok<Option<int>, string>(Option.None<int>()));

        Option<Result<int, string>> transposed = await task.TransposeAsync();

        transposed.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task TransposeAsync_WhenTaskErr_ShouldReturnSomeErr()
    {
        Task<Result<Option<int>, string>> task = Task.FromResult(
            Err<Option<int>, string>(ErrorMessage));

        Option<Result<int, string>> transposed = await task.TransposeAsync();

        transposed.IsSome.Should().BeTrue();
    }

    [Fact]
    public async Task TransposeAsync_WhenValueTaskOkSome_ShouldReturnSomeOk()
    {
        ValueTask<Result<Option<int>, string>> task = ValueTask.FromResult(
            Ok<Option<int>, string>(Option.Some(SuccessValue)));

        Option<Result<int, string>> transposed = await task.TransposeAsync();

        transposed.IsSome.Should().BeTrue();
    }

    [Fact]
    public async Task TransposeAsync_WhenValueTaskOkNone_ShouldReturnNone()
    {
        ValueTask<Result<Option<int>, string>> task = ValueTask.FromResult(
            Ok<Option<int>, string>(Option.None<int>()));

        Option<Result<int, string>> transposed = await task.TransposeAsync();

        transposed.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task TransposeAsync_WhenTaskIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<Option<int>, string>> task = null!;

        Func<Task> act = async () => await task.TransposeAsync();

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
