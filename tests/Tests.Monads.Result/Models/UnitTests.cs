// <copyright file="UnitTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using Monads.Results.Extensions.Sync;
using static Monads.Results.Result;

namespace Monads.Results.Tests;

/// <summary>
/// Contains unit tests for the <see cref="Unit"/> type.
/// </summary>
public sealed class UnitTests
{
    [Fact]
    public void Unit_WhenAccessingDefault_ShouldReturnUnitInstance()
    {
        Unit result = Unit.Default;

        result.Should().Be(default(Unit));
    }

    [Fact]
    public void Unit_WhenComparingTwoInstances_ShouldBeEqual()
    {
        Unit unit1 = default;
        Unit unit2 = Unit.Default;

        unit1.Should().Be(unit2);
        (unit1 == unit2).Should().BeTrue();
    }

    [Fact]
    public void Unit_WhenComparingWithEqualityOperator_ShouldAlwaysReturnTrue()
    {
        Unit unit1 = default;
        Unit unit2 = default;

        (unit1 == unit2).Should().BeTrue();
    }

    [Fact]
    public void Unit_WhenComparingWithInequalityOperator_ShouldAlwaysReturnFalse()
    {
        Unit unit1 = default;
        Unit unit2 = Unit.Default;

        (unit1 != unit2).Should().BeFalse();
    }

    [Fact]
    public void Unit_WhenCallingEquals_ShouldReturnTrue()
    {
        Unit unit1 = default;
        Unit unit2 = Unit.Default;

        unit1.Equals(unit2).Should().BeTrue();
    }

    [Fact]
    public void Unit_WhenCallingEqualsWithObject_ShouldReturnTrueForUnit()
    {
        Unit unit = default;
        object obj = Unit.Default;

        unit.Equals(obj).Should().BeTrue();
    }

    [Fact]
    public void Unit_WhenCallingEqualsWithNonUnitObject_ShouldReturnFalse()
    {
        Unit unit = default;
        object obj = "not a unit";

        unit.Equals(obj).Should().BeFalse();
    }

    [Fact]
    public void Unit_WhenCallingEqualsWithNull_ShouldReturnFalse()
    {
        Unit unit = default;

        unit.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Unit_WhenGettingHashCode_ShouldAlwaysReturnZero()
    {
        Unit unit1 = default;
        Unit unit2 = Unit.Default;

        unit1.GetHashCode().Should().Be(0);
        unit2.GetHashCode().Should().Be(0);
        unit1.GetHashCode().Should().Be(unit2.GetHashCode());
    }

    [Fact]
    public void Unit_WhenCallingToString_ShouldReturnEmptyParentheses()
    {
        Unit unit = default;

        unit.ToString().Should().Be("()");
    }

    [Fact]
    public void Unit_WhenCallingToStringWithFormat_ShouldReturnEmptyParentheses()
    {
        Unit unit = default;

        unit.ToString("G", null).Should().Be("()");
    }

    [Fact]
    public void Unit_WhenConvertingToValueTuple_ShouldSucceed()
    {
        Unit unit = default;

        ValueTuple tuple = unit;

        tuple.Should().Be(default);
    }

    [Fact]
    public void Unit_WhenConvertingFromValueTuple_ShouldSucceed()
    {
        ValueTuple tuple = default;

        Unit unit = tuple;

        unit.Should().Be(Unit.Default);
    }

    [Fact]
    public void Unit_WhenCallingTryFormat_ShouldWriteToSpan()
    {
        Unit unit = default;
        Span<char> buffer = stackalloc char[10];

        bool success = unit.TryFormat(buffer, out int charsWritten, default, null);

        success.Should().BeTrue();
        charsWritten.Should().Be(2);
        buffer[..charsWritten].ToString().Should().Be("()");
    }

    [Fact]
    public void Unit_WhenCallingTryFormatWithSmallBuffer_ShouldFail()
    {
        Unit unit = default;
        Span<char> buffer = stackalloc char[1];

        bool success = unit.TryFormat(buffer, out int charsWritten, default, null);

        success.Should().BeFalse();
        charsWritten.Should().Be(0);
    }

    [Fact]
    public void Unit_WhenUsedInResultType_ShouldRepresentNoValue()
    {
        Result<Unit, string> result = Ok<Unit, string>(Unit.Default);

        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public void Unit_WhenUsedInErrorResult_ShouldWorkCorrectly()
    {
        Result<Unit, string> result = Err<Unit, string>("error");

        result.IsErr.Should().BeTrue();
    }
}
