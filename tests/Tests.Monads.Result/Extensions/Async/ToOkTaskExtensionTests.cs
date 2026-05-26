// <copyright file="ToOkTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="ToOkTaskExtension"/> type.
/// </summary>
public sealed class ToOkTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task ToOkAsync_WhenTaskOk_ShouldReturnSome()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Option<int> option = await resultTask.ToOkAsync();

        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(SuccessValue);
    }

    [Fact]
    public async Task ToOkAsync_WhenTaskErr_ShouldReturnNone()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        Option<int> option = await resultTask.ToOkAsync();

        option.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task ToOkAsync_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = null!;

        Func<Task> act = async () => await resultTask.ToOkAsync();

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
