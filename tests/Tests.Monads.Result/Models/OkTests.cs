// <copyright file="OkTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using static Monads.Results.ResultFactory;

namespace Monads.Results.Tests;

/// <summary>
/// Contains unit tests for the <see cref="Ok{TValue, TError}"/> type.
/// </summary>
public sealed class OkTests
{
    private const int TestValue = 42;

    [Fact]
    public void Ok_WhenConstructedWithValidValue_ShouldContainValue()
    {
        Ok<int, string> result = new(TestValue);

        result.Should().NotBeNull();
        result.Value.Should().Be(TestValue);
    }

    [Fact]
    public void Ok_WhenConstructedWithNullValue_ShouldThrowArgumentNullException()
    {
        Func<Ok<string, string>> act = () => new Ok<string, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ok_WhenCheckedForIsOk_ShouldReturnTrue()
    {
        Ok<int, string> result = new(TestValue);

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenCheckedForIsErr_ShouldReturnFalse()
    {
        Ok<int, string> result = new(TestValue);

        result.IsErr.Should().BeFalse();
    }

    [Fact]
    public void Ok_WhenAccessingValueProperty_ShouldReturnCorrectValue()
    {
        Ok<int, string> result = new(TestValue);

        result.Value.Should().Be(TestValue);
        result.Value.GetType().Should().Be<int>();
    }

    [Fact]
    public void Ok_WhenCreatedViaResultFactory_ShouldCreateOkInstance()
    {
        Result<int, string> result = Success<int, string>(TestValue);

        result.Should().BeOfType<Ok<int, string>>();
        result.IsOk.Should().BeTrue();
        result.IsErr.Should().BeFalse();
    }

    [Fact]
    public void Ok_WhenCreatedViaResultFactoryWithNullValue_ShouldThrowArgumentNullException()
    {
        Func<Result<string, string>> act = () => Success<string, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ok_WhenComparedWithSameValue_ShouldBeEqual()
    {
        Ok<int, string> result1 = new(TestValue);
        Ok<int, string> result2 = new(TestValue);

        result1.Should().Be(result2);
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenComparedWithDifferentValue_ShouldNotBeEqual()
    {
        Ok<int, string> result1 = new(10);
        Ok<int, string> result2 = new(20);

        result1.Should().NotBe(result2);
        (result1 != result2).Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenComparedWithEqualValues_ShouldBeEqualAndHashCodesMatch()
    {
        Ok<string, string> result1 = new(new string("Same value"));
        Ok<string, string> result2 = new(new string("Same value"));

        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Ok_WhenAssignedToResultBase_ShouldBeAssignableAndIsOkTrue()
    {
        Result<int, string> result = new Ok<int, string>(TestValue);

        result.Should().BeOfType<Ok<int, string>>();
        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public void Ok_WhenUsedWithDifferentValueTypes_ShouldMaintainTypeInformation()
    {
        Ok<string, string> stringResult = new("String value");
        Ok<int, string> intResult = new(TestValue);

        stringResult.Value.Should().Be("String value");
        stringResult.Value.Should().BeOfType<string>();
        intResult.Value.Should().Be(42);
        intResult.Value.GetType().Should().Be<int>();
    }
}
