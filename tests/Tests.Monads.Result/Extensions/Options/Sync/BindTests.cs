// <copyright file="BindTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;
using Monads.Options.Extensions.Sync;

namespace Monads.Options.Tests.Extensions.Sync;

/// <summary>
/// Contains unit tests for the <see cref="BindExtension"/> type.
/// </summary>
public sealed class BindTests
{
    private const int TestValue = 42;

    [Fact]
    public void Bind_WhenSelfIsSome_ShouldReturnOperationResult()
    {
        var self = Option.Some(TestValue);

        Option<int> result = self.Bind(value => Option.Some(value + 1));

        result.Should().Be(Option.Some(43));
    }

    [Fact]
    public void Bind_WhenSelfIsSomeAndOperationReturnsNone_ShouldReturnNone()
    {
        var self = Option.Some(TestValue);

        Option<int> result = self.Bind(_ => Option.None<int>());

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Bind_WhenSelfIsNone_ShouldReturnNoneWithoutInvokingOperation()
    {
        var self = Option.None<int>();
        bool invoked = false;

        Option<int> result = self.Bind(value =>
        {
            invoked = true;
            return Option.Some(value);
        });

        result.IsNone.Should().BeTrue();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void Bind_WhenOperationIsNull_ShouldThrowArgumentNullException()
    {
        var self = Option.Some(TestValue);

        Func<Option<int>> act = () => self.Bind<int, int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
