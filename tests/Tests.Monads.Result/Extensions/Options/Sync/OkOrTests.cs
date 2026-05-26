// <copyright file="OkOrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;
using Monads.Results;
using Monads.Results.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="OkOrExtension"/> type.
/// </summary>
public sealed class OkOrTests
{
    private const int TestValue = 42;
    private const string ErrorValue = "missing";

    [Fact]
    public void OkOr_WhenSelfIsSome_ShouldReturnOkOfValue()
    {
        var self = Option.Some(TestValue);

        Result<int, string> result = self.OkOr(ErrorValue);

        result.IsOk.Should().BeTrue();
        result.Match(value => value, _ => -1).Should().Be(TestValue);
    }

    [Fact]
    public void OkOr_WhenSelfIsNone_ShouldReturnErrOfError()
    {
        var self = Option.None<int>();

        Result<int, string> result = self.OkOr(ErrorValue);

        result.IsErr.Should().BeTrue();
        result.Match(_ => string.Empty, error => error).Should().Be(ErrorValue);
    }
}
