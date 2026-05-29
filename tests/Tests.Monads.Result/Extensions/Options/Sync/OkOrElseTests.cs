// <copyright file="OkOrElseTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;
using Monads.Results;
using Monads.Results.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="OkOrElseExtension"/> type.
/// </summary>
public sealed class OkOrElseTests
{
    private const int TestValue = 42;
    private const string ErrorValue = "missing";

    [Fact]
    public void OkOrElse_WhenSelfIsSome_ShouldReturnOkWithoutInvokingError()
    {
        var self = Option.Some(TestValue);
        bool invoked = false;

        Result<int, string> result = self.OkOrElse(() =>
        {
            invoked = true;
            return ErrorValue;
        });

        result.IsOk.Should().BeTrue();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void OkOrElse_WhenSelfIsNone_ShouldReturnErrOfErrorResult()
    {
        var self = Option.None<int>();

        Result<int, string> result = self.OkOrElse(() => ErrorValue);

        result.IsErr.Should().BeTrue();
        result.Match(_ => string.Empty, error => error).Should().Be(ErrorValue);
    }

    [Fact]
    public void OkOrElse_WhenErrorIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Result<int, string>> act = () => self.OkOrElse<int, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
