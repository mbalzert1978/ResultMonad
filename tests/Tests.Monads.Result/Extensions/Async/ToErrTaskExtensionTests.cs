// <copyright file="ToErrTaskExtensionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Results;
using Monads.Results.Extensions.Async;
using static Monads.Results.Result;

namespace Monads.Results.Tests.Extensions.Async;

/// <summary>
/// Contains unit tests for the <see cref="ToErrTaskExtension"/> type.
/// </summary>
public sealed class ToErrTaskExtensionTests
{
    private const int SuccessValue = 42;
    private const string ErrorMessage = "boom";

    [Fact]
    public async Task ToErrAsync_WhenTaskErr_ShouldReturnSome()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Err<int, string>(ErrorMessage));

        Option<string> option = await resultTask.ToErrAsync();

        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(ErrorMessage);
    }

    [Fact]
    public async Task ToErrAsync_WhenTaskOk_ShouldReturnNone()
    {
        Task<Result<int, string>> resultTask = Task.FromResult(Ok<int, string>(SuccessValue));

        Option<string> option = await resultTask.ToErrAsync();

        option.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task ToErrAsync_WhenSelfIsNull_ShouldThrowArgumentNullException()
    {
        Task<Result<int, string>> resultTask = null!;

        Func<Task> act = async () => await resultTask.ToErrAsync();

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
