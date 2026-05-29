// <copyright file="OptionTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Options;

namespace Monads.Options.Tests;

/// <summary>
/// Specifies the public API contract for the planned <see cref="Option{T}"/> readonly record struct.
/// These tests reference only the post-refactor surface — <c>Option.Some(value)</c>, <c>Option.None&lt;T&gt;()</c>,
/// and instance members <c>IsSome</c>, <c>IsNone</c>, <c>Value</c>, <c>Match</c>. They are expected to be
/// RED until the implementation in #12 lands.
/// </summary>
public sealed class OptionTests
{
    private const int TestValue = 42;
    private const string TestString = "hello";

    [Fact]
    public void Some_WhenConstructedWithValidValue_ShouldExposeIsSomeTrue()
    {
        var option = Option.Some(TestValue);

        option.IsSome.Should().BeTrue();
        option.IsNone.Should().BeFalse();
    }

    [Fact]
    public void Some_WhenConstructedWithValidValue_ShouldExposeWrappedValue()
    {
        var option = Option.Some(TestValue);

        option.Value.Should().Be(TestValue);
    }

    [Fact]
    public void Some_WhenConstructedWithNullValue_ShouldThrowArgumentNullException()
    {
        Func<Option<string>> act = () => Option.Some<string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void None_WhenConstructedExplicitly_ShouldExposeIsNoneTrue()
    {
        var option = Option.None<int>();

        option.IsNone.Should().BeTrue();
        option.IsSome.Should().BeFalse();
    }

    [Fact]
    public void None_WhenConstructedExplicitly_ShouldReturnDefaultValue()
    {
        var option = Option.None<string>();

        option.Value.Should().BeNull();
    }

    [Fact]
    public void Default_WhenInstantiated_ShouldRepresentNone()
    {
        Option<int> option = default;

        option.IsNone.Should().BeTrue();
        option.IsSome.Should().BeFalse();
    }

    [Fact]
    public void Value_WhenIsSomeIsTrue_ShouldBeTreatedAsNonNullable()
    {
        var option = Option.Some(TestString);

        if (option.IsSome)
        {
            // No '!' operator, no '?.', no #pragma — flow analysis from
            // [MemberNotNullWhen(true, nameof(Value))] on IsSome must narrow
            // Value to non-nullable string under TreatWarningsAsErrors.
            string value = option.Value;

            value.Should().Be(TestString);
        }
        else
        {
            Assert.Fail("Expected option to be Some.");
        }
    }

    [Fact]
    public void Match_WhenSome_ShouldInvokeOnSomeBranchAndReturnResult()
    {
        var option = Option.Some(TestValue);

        string matched = option.Match(value => $"value:{value}", () => "none");

        matched.Should().Be("value:42");
    }

    [Fact]
    public void Match_WhenNone_ShouldInvokeOnNoneBranchAndReturnResult()
    {
        var option = Option.None<int>();

        string matched = option.Match(value => $"value:{value}", () => "none");

        matched.Should().Be("none");
    }

    [Fact]
    public void Match_WhenOnSomeReturnsNull_ShouldThrowInvalidOperationException()
    {
        var option = Option.Some(TestValue);

        Func<string> act = () => option.Match(_ => null!, () => "none");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Match_WhenOnNoneReturnsNull_ShouldThrowInvalidOperationException()
    {
        var option = Option.None<int>();

        Func<string> act = () => option.Match(value => $"value:{value}", () => null!);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equality_WhenBothSomeWithSameValue_ShouldBeEqual()
    {
        var a = Option.Some(TestValue);
        var b = Option.Some(TestValue);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Equality_WhenBothNone_ShouldBeEqual()
    {
        var a = Option.None<int>();
        var b = Option.None<int>();

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_WhenDefaultAndExplicitNone_ShouldBeEqual()
    {
        Option<int> a = default;
        var b = Option.None<int>();

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_WhenSomeZeroAndNoneOfSameValueType_ShouldNotBeEqual()
    {
        var some = Option.Some(0);
        var none = Option.None<int>();

        some.Should().NotBe(none);
        (some == none).Should().BeFalse();
        (some != none).Should().BeTrue();
    }

    [Fact]
    public void Equality_WhenBothSomeWithDifferentValues_ShouldNotBeEqual()
    {
        var a = Option.Some(1);
        var b = Option.Some(2);

        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WhenBothSomeWithEqualValues_ShouldMatch()
    {
        var a = Option.Some(new string("same"));
        var b = Option.Some(new string("same"));

        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
