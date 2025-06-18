# Use Cases for ConsoleApp

## Use Case 1: Greet Command

- Greet a user by name via the command line.
- Example: `dotnet run --project ConsoleApp greet Alice` outputs `Hello, Alice!`

## Use Case 2: Add Command

- Add two numbers provided as arguments.
- Example: `dotnet run --project ConsoleApp add 2 3` outputs `Sum: 5`

## Use Case 3: Help Command

- Show usage and available commands.
- Example: `dotnet run --project ConsoleApp help` outputs usage info and command list.

## Use Case 4: Invalid Command Handling

- Show error and help suggestion for unknown commands.
- Example: `dotnet run --project ConsoleApp foo` outputs error and help tip.
