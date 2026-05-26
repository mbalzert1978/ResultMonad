// <copyright file="OkOrValueTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Async;
using Monads.Results;
using Monads.Results.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="OkOrValueTaskExtension"/> type.
/// </summary>
public sealed class OkOrValueTaskExtensionTests
{
    private const int TestValue = 42;
    private const string ErrorValue = "missing";

    [Fact]
    public async Task OkOrAsync_WhenValueTaskSome_ShouldReturnOk()
    {
        ValueTask<Option<int>> selfTask = new(Option.Some(TestValue));

        Result<int, string> result = await selfTask.OkOrAsync(ErrorValue);

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public async Task OkOrAsync_WhenValueTaskNone_ShouldReturnErrOfError()
    {
        ValueTask<Option<int>> selfTask = new(Option.None<int>());

        Result<int, string> result = await selfTask.OkOrAsync(ErrorValue);

        result.IsErr.Should().BeTrue();
    }
}
