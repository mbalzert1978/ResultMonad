// <copyright file="OkOrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;
using Monads.Results;
using Monads.Results.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OkOrTaskExtension"/> type.
/// </summary>
public sealed class OkOrTaskExtensionTests
{
    private const int TestValue = 42;
    private const string ErrorValue = "missing";

    [Fact]
    public async Task OkOrAsync_WhenTaskSome_ShouldReturnOk()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.Some(TestValue));

        Result<int, string> result = await selfTask.OkOrAsync(ErrorValue);

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrAsync_WhenTaskNone_ShouldReturnErrOfError()
    {
        Task<Option<int>> selfTask = Task.FromResult(Option.None<int>());

        Result<int, string> result = await selfTask.OkOrAsync(ErrorValue);

        result.IsErr.Should().BeTrue();
        result.Match(_ => string.Empty, e => e).Should().Be(ErrorValue);
    }

    [Fact]
    public async Task OkOrAsync_WhenTaskReceiverIsNull_ShouldThrowArgumentNullException()
    {
        Task<Option<int>> selfTask = null!;

        Func<Task> act = async () => await selfTask.OkOrAsync(ErrorValue);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
