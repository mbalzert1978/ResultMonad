# Documentation Standards

This document defines the documentation standards for the Monads project to ensure consistency, clarity, and maintainability across all documentation.

## XML Documentation Comments (Code)

### Required Tags

All public, internal, and private code elements must have XML documentation comments with the following tags:

#### Classes, Structs, Records, Interfaces

- `<summary>`: Brief description of the type's purpose (1-2 sentences)
- `<typeparam>`: Description for each generic type parameter
- `<remarks>`: Additional context, usage notes, or design decisions (optional but recommended)
- `<example>`: Code example showing typical usage (for complex or non-intuitive types)

**Example:**

```csharp
/// <summary>
/// Represents a result that can be either a success value or an error value.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error value.</typeparam>
/// <remarks>
/// This type implements the Result monad pattern, allowing for railway-oriented programming
/// and explicit error handling without exceptions.
/// </remarks>
public abstract record Result<T, E>
    where T : notnull
    where E : notnull
{
    // ...
}
```

#### Methods and Extension Methods

- `<summary>`: What the method does (action-oriented, present tense)
- `<typeparam>`: Description for each generic type parameter
- `<param>`: Description for each parameter
- `<returns>`: Description of the return value
- `<exception>`: All possible exceptions that can be thrown with conditions
- `<example>`: Code example showing usage (required for complex methods)
- `<remarks>`: Performance considerations, thread-safety, or important notes (optional)

**Example:**

```csharp
/// <summary>
/// Transforms the success value of a result using the specified mapping function.
/// </summary>
/// <typeparam name="T">The type of the source success value.</typeparam>
/// <typeparam name="E">The type of the error value.</typeparam>
/// <typeparam name="U">The type of the target success value.</typeparam>
/// <param name="result">The source result to transform.</param>
/// <param name="mapper">The function to apply to the success value.</param>
/// <returns>
/// A new result with the transformed success value if the source is Ok;
/// otherwise, returns the original error.
/// </returns>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="mapper"/> is <c>null</c>.
/// </exception>
/// <example>
/// <code>
/// var result = Ok&lt;int, string&gt;(42);
/// var mapped = result.Map(x => x * 2);
/// // mapped is Ok(84)
/// 
/// var error = Err&lt;int, string&gt;("Failed");
/// var mappedError = error.Map(x => x * 2);
/// // mappedError is still Err("Failed")
/// </code>
/// </example>
public static Result<U, E> Map<T, E, U>(
    this Result<T, E> result,
    Func<T, U> mapper)
    where T : notnull
    where E : notnull
    where U : notnull
{
    // ...
}
```

#### Properties

- `<summary>`: Description of what the property represents
- `<value>`: Description of the property value (for non-obvious properties)
- `<exception>`: Exceptions that can be thrown by the getter or setter

**Example:**

```csharp
/// <summary>
/// Gets a value indicating whether this result represents a success.
/// </summary>
/// <value>
/// <c>true</c> if this instance is <see cref="Ok{T,E}"/>; otherwise, <c>false</c>.
/// </value>
public abstract bool IsOk { get; }
```

#### Fields and Constants

- `<summary>`: Brief description of the field/constant and its purpose

**Example:**

```csharp
/// <summary>
/// Error message used when a null mapper function is provided.
/// </summary>
private const string NullMapperError = "Mapper function cannot be null.";
```

### Style Guidelines for XML Comments

1. **Be Concise**: Keep `<summary>` to 1-2 sentences
2. **Use Present Tense**: "Gets the value" not "Will get the value"
3. **Be Action-Oriented**: "Transforms the result" not "This method transforms"
4. **Avoid Redundancy**: Don't repeat the method name
5. **Reference Other Types**: Use `<see cref="TypeName"/>` to link to related types
6. **Use `<c>` for Inline Code**: Wrap keywords, values, or type names: `<c>null</c>`, `<c>true</c>`
7. **Use `<code>` for Examples**: Multi-line examples go in `<code>` blocks
8. **Document Edge Cases**: Mention behavior for null, empty, or boundary conditions
9. **Be Explicit About Exceptions**: Always document when and why exceptions are thrown

### Async Method Documentation

For async methods, include:

- `<remarks>` explaining async behavior and awaiting requirements
- Performance implications of Task vs ValueTask
- Cancellation token support (if applicable)

**Example:**

```csharp
/// <summary>
/// Asynchronously transforms the success value of a Task-wrapped result.
/// </summary>
/// <remarks>
/// This method awaits the source task and then applies the mapper function.
/// The mapper function itself is synchronous. For async mapping operations,
/// use BindAsync instead.
/// </remarks>
```

---

## Markdown Documentation Files

### File Structure

Each Markdown file should follow this structure:

1. **Title (H1)**: Clear, descriptive title
2. **Brief Introduction**: 1-2 paragraphs explaining the topic
3. **Main Sections (H2)**: Logical grouping of content
4. **Subsections (H3, H4)**: Detailed breakdowns as needed
5. **Examples**: Code examples in fenced code blocks
6. **See Also**: Links to related documentation

### Style Guidelines

#### Tone and Voice

- **Clear and Precise**: Technical accuracy is paramount
- **Friendly and Inviting**: Welcoming to beginners
- **Explicit**: Make assumptions clear, don't leave readers guessing
- **Educational**: Explain the "why" not just the "what"

#### Writing Style

- **Short Paragraphs**: Maximum 3-4 sentences per paragraph
- **Use Lists**: For enumerations, steps, or options
- **Active Voice**: Prefer "You can use" over "It can be used"
- **Present Tense**: "The method returns" not "The method will return"

#### Code Examples

- **Fenced Code Blocks**: Always use triple backticks with language identifier

  ````markdown
  ```csharp
  var result = Ok<int, string>(42);
  ```
  ````

- **Complete and Runnable**: Examples should be complete enough to understand
- **Commented**: Add comments to explain non-obvious parts
- **Realistic**: Show real-world scenarios, not just trivial examples
- **Progressive**: Start simple, then show more advanced usage

#### Formatting

- **Bold** for emphasis on key terms (first usage)
- _Italic_ for introducing terminology
- `Inline code` for type names, method names, keywords, file paths
- Links for cross-references: `[See API Reference](../api-reference/models/result.md)`

#### Headers

- Use sentence case: "Getting started" not "Getting Started"
- Be descriptive: "How to handle errors" not just "Errors"
- Consistent hierarchy: Don't skip levels (H1 → H2 → H3)

### API Reference Template

API reference documents should follow this template:

```markdown
# [Type/Method Name]

Brief one-line description.

## Syntax

```csharp
// Full method signature
public static Result<U, E> MethodName<T, E, U>(...)
```

## Type Parameters

- `T`: Description of type parameter T
- `E`: Description of type parameter E

## Parameters

- `paramName`: Description of what this parameter represents

## Returns

Description of the return value and its possible states.

## Exceptions

- `ExceptionType`: When and why this exception is thrown

## Remarks

Additional information about behavior, performance, thread-safety, etc.

## Basic Examples

### Basic Usage

```csharp
// Example code with comments
var result = Ok<int, string>(42);
var transformed = result.Method(...);
```

### Error Handling

```csharp
// Example showing error case
var error = Err<int, string>("Failed");
var transformed = error.Method(...);
// Result: still Err("Failed")
```

## See Also

- [Related Type A](./related-type-a.md)
- [Related Method B](./related-method-b.md)

### Cross-Referencing

- Link to related concepts, API references, and examples
- Use relative paths for internal documentation
- Ensure all links are valid (will be checked by CI/CD)

**Example:**

```markdown
For more information about error handling patterns, see [Error Handling Guide](../concepts/error-handling.md).

The async variant is available as [MapAsync](./async-extensions.md#mapasync).
```

---

## Commit Message Standards

When committing documentation changes, follow these guidelines:

### Format

Use conventional commit format:

```bash
docs: brief description of change

- Detailed bullet points of what was added/changed
- Reference to related tasks or PRDs

Related to: Task X.Y in PRD-NNNN
```

### Doc Examples

```bash
docs: add XML docstrings for Result model

- Add comprehensive summary, typeparam, and remarks tags
- Include code examples for Ok and Err usage
- Document all public properties and methods

Related to: Task 2.2 in PRD-0001
```

```bash
docs: create getting started guide

- Add installation instructions
- Include quick start example
- Link to basic concepts documentation

Related to: Task 4.1 in PRD-0001
```

---

## Quality Checklist

Before considering documentation complete, verify:

### XML Documentation

- [ ] All public/internal/private members have `<summary>` tags
- [ ] All methods have `<param>` and `<returns>` tags
- [ ] All generic parameters have `<typeparam>` tags
- [ ] All exceptions are documented with `<exception>` tags
- [ ] Complex methods have `<example>` tags with runnable code
- [ ] No CS1591 warnings in build output

### Markdown Documentation

- [ ] All sections have clear, descriptive headers
- [ ] Code examples use proper syntax highlighting
- [ ] Code examples are complete and runnable
- [ ] All internal links are valid (no broken links)
- [ ] Cross-references are bidirectional where appropriate
- [ ] Terminology is used consistently
- [ ] No spelling or grammar errors

### Completeness

- [ ] All user stories from PRD are addressed
- [ ] All functional requirements are documented
- [ ] Getting started guide enables quick onboarding
- [ ] API reference covers all public APIs
- [ ] Examples show common use cases

---

## Maintenance

Documentation should be updated when:

1. **New features are added**: Update API reference and examples
2. **APIs are changed**: Update signatures, examples, and migration guides
3. **Bugs are fixed**: Update examples if they showed incorrect usage
4. **Patterns emerge**: Add to examples and best practices
5. **User feedback**: Address confusion or gaps in documentation

CI/CD will validate:

- XML documentation completeness (CS1591 warnings)
- Markdown link validity
- Code example syntax (optional: compilation checks)

---

## Resources

- [Microsoft C# XML Documentation Guidelines](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)
- [GitHub Markdown Guide](https://guides.github.com/features/mastering-markdown/)
- [Write the Docs - Documentation Guide](https://www.writethedocs.org/guide/)

---

**Last Updated**: 12. Oktober 2025
