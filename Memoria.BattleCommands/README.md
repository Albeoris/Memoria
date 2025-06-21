# Memoria.BattleCommands

A refactored battle command system library for the Memoria project, providing clean, testable, and maintainable battle command functionality.

## Overview

This library contains the extracted and refactored battle command logic from the original monolithic `btl_cmd` class. It provides a clean separation between pure business logic and game engine dependencies, enabling comprehensive unit testing and improved maintainability.

## Features

- **Pure .NET 3.5 Library**: No Unity dependencies, making it reusable and testable
- **Comprehensive API**: Core battle command utilities with full XML documentation
- **Unit Tested**: 27+ test cases covering all scenarios and edge cases
- **Zero Breaking Changes**: Seamless integration with existing codebase
- **Extensible Architecture**: Framework ready for additional command functionality

## API Reference

### BattleCommandService

Core static service providing battle command utilities:

- `ClearCommand(CommandData)` - Resets command to default state
- `ClearReflectionData(CommandData)` - Clears reflection targets
- `ValidateCommand(CommandData)` - Validates command for execution
- `CreateCommand()` - Factory method for new commands
- `CopyCommand(CommandData)` - Deep copy utility

### Data Types

- `CommandData` - Main command data structure
- `CommandInfo` - Command execution and selection information
- `ReflectionData` - Reflection target data for magical commands
- `BattleCommandId` - Command type enumeration

## Usage

```csharp
using Memoria.BattleCommands.Core;
using Memoria.BattleCommands.Data;

// Create a new command
var command = BattleCommandService.CreateCommand();
command.CommandId = BattleCommandId.Attack;
command.TargetId = 1;

// Validate and process
if (BattleCommandService.ValidateCommand(command))
{
    // Use the command...
    BattleCommandService.ClearReflectionData(command);
}

// Create a copy
var commandCopy = BattleCommandService.CopyCommand(command);
```

## Integration

The library integrates seamlessly with the existing Memoria codebase through the `CommandDataAdapter` class, which handles conversion between the original `CMD_DATA` objects and the new `CommandData` objects.

## Testing

Run the test suite with:
```bash
dotnet test Memoria.BattleCommands.Tests
```

The test suite includes:
- Unit tests for all public methods
- Edge case validation
- Error condition handling
- Data integrity verification

## Contributing

When adding new functionality:

1. Write comprehensive tests first
2. Add XML documentation to all public members
3. Follow the established patterns for data conversion
4. Maintain backward compatibility with the original API

## License

This code is part of the Memoria project and follows the same licensing terms.