# Technical Requirements

- dotnet core latest version
- Visual Studio Code or any other IDE that supports C# development
- Basic understanding of C# and .NET Core
- Familiarity with console applications and command-line arguments
- Ability to run and test console applications
- Ensure that everything is setup via the clean archtecture approach
- MAke sure all class files are in theri own file with the same name as the class
- Use dependency injection
- Use the Mediatr pattern
- Make sure that the application can be executed from VS Code and Visual Studio as a solution

## Sample Usage

### Greet Command

```sh
dotnet run --project ConsoleApp greet Alice
```

**Output:**

```text
Hello, Alice!
```

### Add Command

```sh
dotnet run --project ConsoleApp add 2 3
```

**Output:**

```text
Sum: 5
```

### Help Command

```sh
dotnet run --project ConsoleApp help
```

**Output:**

```text
Usage: ConsoleApp <command> [options]
Commands:
  greet <name>   - Greet the user by name
  add <a> <b>    - Add two numbers
```

### Invalid Command

```sh
dotnet run --project ConsoleApp foo
```

**Output:**

```text
Unknown command: foo
Try 'help' for a list of commands.
```

### Running Tests

```sh
# From solution/ConsoleApp folder, run all tests:
dotnet test src\\ConsoleApp.Tests\\ConsoleApp.Tests.csproj
```

## Error Handling for Missing Dependencies

If a required dependency (like MediatR or DI setup) is missing or misconfigured, the app will now show a clear error message and exit. This helps users quickly identify and fix setup issues.
