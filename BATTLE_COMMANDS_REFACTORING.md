# Memoria.BattleCommands Refactoring Documentation

## Overview

This document describes the completed refactoring of battle command utilities from the monolithic `btl_cmd` class into a separate, testable, and maintainable library. This serves as a proof-of-concept and template for refactoring the remaining 1600+ lines of the `btl_cmd` class.

## Architecture

### 1. Memoria.BattleCommands Library (.NET 3.5)

**Purpose**: Pure business logic for battle commands without Unity dependencies.

**Key Components**:
- `BattleCommandService`: Core static service with extracted utilities
- `CommandData`: Clean data model replacing CMD_DATA for core operations
- `CommandInfo`: Command execution and selection information
- `ReflectionData`: Reflection target data for magical commands
- `BattleCommandId`: Simplified command type enumeration

**Benefits**:
- ✅ Zero Unity dependencies - pure C# logic
- ✅ Fully unit testable with mocks and fakes
- ✅ Can be reused in other contexts (tools, server-side logic)
- ✅ Clear separation of concerns
- ✅ Comprehensive XML documentation

### 2. Integration Layer

**Purpose**: Seamless bridge between legacy code and new library.

**Key Components**:
- `CommandDataAdapter`: Converts between CMD_DATA and CommandData
- Updated `btl_cmd` methods that delegate to new service

**Benefits**:
- ✅ Zero breaking changes to existing code
- ✅ Gradual migration path for remaining methods
- ✅ Preserves all original functionality

### 3. Comprehensive Test Suite

**Purpose**: Ensures correctness and enables confident refactoring.

**Coverage**:
- 27 test cases covering all scenarios
- Edge cases and error conditions
- Null safety validation
- Data integrity verification

## Extracted Methods

### ✅ Completed

1. **`ClearCommand`** - Resets command to default state
2. **`ClearReflectionData`** - Clears reflection targets
3. **`ValidateCommand`** - Validates command for execution
4. **`CreateCommand`** - Factory method for new commands
5. **`CopyCommand`** - Deep copy utility

### 🔄 Remaining (Future Work)

The following methods from the original 1665-line `btl_cmd` class can be refactored using the same pattern:

1. **Command Management**:
   - `InitCommandSystem` - Initialize battle command system
   - `EnqueueCommand` - Add command to execution queue
   - `ManageDequeueCommand` - Process command queue
   - `CommandEngine` - Main command processing loop

2. **Command Execution**:
   - `FinishCommand` - Complete command execution
   - `ReqFinishCommand` - Request command completion
   - `ExecVfxCommand` - Execute visual effects

3. **Command Validation**:
   - `CheckCommandCondition` - Validate command can execute
   - `CheckMagicCondition` - Validate magic command requirements
   - `CheckMpCondition` - Validate MP requirements
   - `CheckTargetCondition` - Validate target availability

4. **Specialized Commands**:
   - `CheckReflec` - Handle spell reflection
   - `KillCommand` - Cancel commands
   - `KillAllCommand` - Cancel all commands
   - `InsertCommand` - Insert priority commands

## Usage Examples

### Original Code
```csharp
// Old way - direct static calls with tight coupling
CMD_DATA cmd = new CMD_DATA();
btl_cmd.ClearCommand(cmd);
btl_cmd.ClearReflecData(cmd);
```

### Refactored Code (Internal)
```csharp
// New way - through adapter (maintains compatibility)
CMD_DATA cmd = new CMD_DATA();
btl_cmd.ClearCommand(cmd);  // Now uses BattleCommandService internally
btl_cmd.ClearReflecData(cmd);  // Now uses BattleCommandService internally
```

### Direct Use of New Library
```csharp
// Direct use of new library (for new code)
using Memoria.BattleCommands.Core;
using Memoria.BattleCommands.Data;

var command = BattleCommandService.CreateCommand();
command.CommandId = BattleCommandId.Attack;
command.TargetId = 1;

if (BattleCommandService.ValidateCommand(command))
{
    BattleCommandService.ClearReflectionData(command);
    // Process command...
}
```

### Unit Testing
```csharp
[Fact]
public void ClearCommand_ValidCommand_ClearsAllProperties()
{
    // Arrange
    var command = new CommandData 
    { 
        CommandId = BattleCommandId.Attack,
        TargetId = 123 
    };
    
    // Act
    BattleCommandService.ClearCommand(command);
    
    // Assert
    Assert.Equal(BattleCommandId.None, command.CommandId);
    Assert.Equal(0u, command.TargetId);
}
```

## Migration Strategy

### Phase 1: Foundation ✅ COMPLETE
- Create new library project
- Extract simple utility methods
- Create test infrastructure
- Establish integration pattern

### Phase 2: Command Management (Next)
1. Extract `InitCommandSystem` with dependency injection for FF9StateBattleSystem
2. Create `IBattleSystemState` interface
3. Move command queue management logic
4. Add tests for initialization scenarios

### Phase 3: Command Validation
1. Extract validation methods with dependency injection
2. Create `IBattleDataProvider` interface for battle state access
3. Create `IMagicSystemProvider` interface for MP/magic validation
4. Add comprehensive validation tests

### Phase 4: Command Execution
1. Extract execution methods with complex dependency injection
2. Create `IBattleEffectProcessor` interface for visual effects
3. Create `IBattleCalculator` interface for damage/healing calculations
4. Add execution flow tests

### Phase 5: Specialized Commands
1. Extract specialized command handling
2. Create command-specific interfaces and services
3. Add specialized command tests

## Benefits Achieved

### ✅ Testability
- Pure unit tests without Unity runtime
- Mock dependencies for isolated testing
- Fast test execution (no game engine overhead)
- Easy to test edge cases and error conditions

### ✅ Maintainability
- Clear separation of concerns
- Well-documented public API
- Consistent error handling
- Type safety improvements

### ✅ Reusability
- Can be used in tools and utilities
- Server-side logic for multiplayer scenarios
- Command validation in save file tools
- Battle simulation and testing tools

### ✅ Zero Breaking Changes
- Original `btl_cmd` interface preserved
- Gradual migration possible
- No impact on existing save files
- No Unity serialization issues

## Development Guidelines

### When Adding New Methods

1. **Extract Logic**: Move pure command logic to `BattleCommandService`
2. **Create Interfaces**: Define interfaces for external dependencies
3. **Add Tests**: Comprehensive test coverage for all scenarios
4. **Document**: XML documentation for all public members
5. **Integrate**: Update original method to use new service via adapter

### Interface Design Principles

1. **Game-Agnostic**: Interfaces should not reference Unity or FF9-specific types
2. **Minimal**: Only include methods actually needed by command logic
3. **Focused**: Single responsibility per interface
4. **Mockable**: Easy to create test implementations

### Testing Standards

1. **Comprehensive**: Test normal cases, edge cases, and error conditions
2. **Isolated**: Use mocks for all external dependencies
3. **Fast**: Tests should execute quickly without I/O
4. **Descriptive**: Test names should clearly indicate the scenario being tested

## Conclusion

This refactoring establishes a solid foundation for modernizing the battle command system. The pattern demonstrated here can be systematically applied to the remaining methods in `btl_cmd`, gradually transforming a 1665-line monolithic class into a well-structured, testable, and maintainable system.

The key success factors are:
- Preserving backward compatibility
- Comprehensive testing
- Clean architectural separation
- Gradual migration approach

This approach minimizes risk while maximizing the long-term benefits of modern software engineering practices.