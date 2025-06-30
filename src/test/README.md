# CDP Test Project

This project contains unit tests for the ei8.Cortex.Diary.Plugins.Tree library.

## Project Structure

- `TreeQueryTests.cs` - Comprehensive unit tests for the `TreeQuery` class
- `cdp.test.csproj` - Project file with test dependencies
- `README.md` - This documentation file

## Test Coverage

The `TreeQueryTests.cs` file includes tests for:

### TreeQuery Constructor Tests

- Default constructor behavior
- Property initialization

### ToString Method Tests

- Empty query string generation
- Single parameter query string generation
- Multiple parameter query string generation
- Enum value serialization
- All properties serialization

### TryParse Method Tests

- Valid query string parsing
- Empty/null/whitespace string handling
- URL-encoded value decoding
- Multiple value parsing
- Enum value parsing
- Numeric value parsing
- Boolean value parsing
- Avatar URL parameter extraction
- Complex avatar URL handling
- Invalid query string handling
- Query string with/without question mark
- JSON serialized object parsing

## Running Tests

### Using .NET CLI

```bash
# Navigate to the test directory
cd test/cdp.test

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Using Visual Studio

1. Open the `cdp.test.sln` solution file
2. Build the solution
3. Open Test Explorer (Test > Test Explorer)
4. Run all tests or individual test methods

## Dependencies

- **xUnit** - Testing framework
- **Microsoft.NET.Test.Sdk** - Test SDK
- **coverlet.collector** - Code coverage collection
- **Tree.csproj** - Reference to the main project being tested

## Test Data

The tests use various sample data including:

- Simple string values
- URL-encoded strings
- Enum values (SortByValue, SortOrderValue, ActiveValues, etc.)
- Numeric values
- Boolean values
- JSON serialized objects
- Avatar URLs with embedded query parameters

## Notes

- Tests are designed to be independent and can run in any order
- Each test method focuses on a specific aspect of the TreeQuery class
- Error handling scenarios are covered
- Edge cases like empty strings, null values, and malformed query strings are tested
