// <copyright file="ErrTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using ResultMonad;

namespace Tests.ResultMonad;

/// <summary>
/// Contains unit tests for the <see cref="Err{TValue, TError}"/> type.
/// </summary>
public sealed class ErrTests
{
    private const string ErrorMessage = "Test error message";

    [Fact]
    public void Err_WhenConstructedWithValidError_ShouldContainErrorValue()
    {
        Err<int, string> result = new(ErrorMessage);

        result.Should().NotBeNull();
        result.Error.Should().Be(ErrorMessage);
    }

    [Fact]
    public void Err_WhenConstructedWithNullError_ShouldThrowArgumentNullException()
    {
        Func<Err<int, string>> act = () => new Err<int, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Err_WhenCheckedForIsOk_ShouldReturnFalse()
    {
        Err<int, string> result = new(ErrorMessage);

        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void Err_WhenCheckedForIsErr_ShouldReturnTrue()
    {
        Err<int, string> result = new(ErrorMessage);

        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public void Err_WhenAccessingErrorProperty_ShouldReturnCorrectError()
    {
        Err<int, string> result = new(ErrorMessage);

        result.Error.Should().Be(ErrorMessage);
        result.Error.Should().BeOfType<string>();
    }

    [Fact]
    public void Err_WhenCreatedViaResultFactory_ShouldCreateErrInstance()
    {
        Result<int, string> result = ResultFactory.Err<int, string>(ErrorMessage);

        result.Should().BeOfType<Err<int, string>>();
        result.IsErr.Should().BeTrue();
        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void Err_WhenCreatedViaResultFactoryWithNullError_ShouldThrowArgumentNullException()
    {
        Func<Result<int, string>> act = () => ResultFactory.Err<int, string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Err_WhenComparedWithSameError_ShouldBeEqual()
    {
        // Arrange
        string error = ErrorMessage;
        Err<int, string> result1 = new(error);
        Err<int, string> result2 = new(error);

        // Act & Assert
        result1.Should().Be(result2);
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Err_WhenComparedWithDifferentError_ShouldNotBeEqual()
    {
        // Arrange
        string error1 = new("Error 1");
        string error2 = new("Error 2");
        Err<int, string> result1 = new(error1);
        Err<int, string> result2 = new(error2);

        // Act & Assert
        result1.Should().NotBe(result2);
        (result1 != result2).Should().BeTrue();
    }

    [Fact]
    public void Err_WhenComparedWithEqualErrorValues_ShouldBeEqualAndHashCodesMatch()
    {
        // Arrange
        string error1 = new("Same message");
        string error2 = new("Same message");
        Err<int, string> result1 = new(error1);
        Err<int, string> result2 = new(error2);

        // Act & Assert
        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Err_WhenAssignedToResultBase_ShouldBeAssignableAndIsErrTrue()
    {
        // Arrange
        string error = "Error";
        Err<int, string> err = new(error);

        // Act
        Result<int, string> result = err;

        result.Should().BeOfType<Err<int, string>>();
        result.IsErr.Should().BeTrue();
    }

    [Fact]
    public void Err_WhenUsedWithDifferentErrorTypes_ShouldMaintainTypeInformation()
    {
        // Arrange
        string stringError = "String error";
        int intError = 42;

        // Act
        Err<int, string> stringResult = new(stringError);
        Err<int, int> intResult = new(intError);

        stringResult.Error.Should().Be(stringError);
        stringResult.Error.Should().BeOfType<string>();
        intResult.Error.Should().Be(42);
        intResult.Error.GetType().Should().Be<int>();
    }
}
