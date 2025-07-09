using System;
using Xunit;
using Memoria.BattleCommands.Core;
using Memoria.BattleCommands.Data;

namespace Memoria.BattleCommands.Tests.Core
{
    /// <summary>
    /// Unit tests for the BattleCommandService class.
    /// </summary>
    public class BattleCommandServiceTests
    {
        #region ClearCommand Tests

        [Fact]
        public void ClearCommand_ValidCommand_ClearsAllProperties()
        {
            // Arrange
            var command = new CommandData
            {
                Next = new CommandData(),
                TargetId = 123,
                MagicCasterId = 456,
                CommandId = BattleCommandId.Attack,
                SubNumber = 5,
                IsShortRange = true,
                HitRate = 95,
                Power = 100,
                ScriptId = 42
            };
            
            // Set some info properties to non-default values
            command.Info.Cursor = 3;
            command.Info.Status = 7;
            command.Info.Priority = 2;
            command.Info.CustomMPCost = 25;

            // Act
            BattleCommandService.ClearCommand(command);

            // Assert
            Assert.Null(command.Next);
            Assert.Equal((UInt16)0, command.TargetId);
            Assert.Equal((UInt16)0, command.MagicCasterId);
            Assert.Equal(BattleCommandId.None, command.CommandId);
            Assert.Equal(0, command.SubNumber);
            
            // Verify Info was reset (checking a few key properties)
            Assert.Equal((Byte)0, command.Info.Cursor);
            Assert.Equal((Byte)0, command.Info.Status);
            Assert.Equal((Byte)0, command.Info.Priority);
            Assert.Null(command.Info.CustomMPCost);
            Assert.False(command.Info.CommandMotion);
            Assert.False(command.Info.ReflectNull);
        }

        [Fact]
        public void ClearCommand_NullCommand_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => BattleCommandService.ClearCommand(null));
            Assert.Equal("command", exception.ParamName);
        }

        [Fact]
        public void ClearCommand_AlreadyCleanCommand_RemainsClean()
        {
            // Arrange
            var command = new CommandData();

            // Act
            BattleCommandService.ClearCommand(command);

            // Assert - should still be in clean state
            Assert.Null(command.Next);
            Assert.Equal((UInt16)0, command.TargetId);
            Assert.Equal((UInt16)0, command.MagicCasterId);
            Assert.Equal(BattleCommandId.None, command.CommandId);
            Assert.Equal(0, command.SubNumber);
        }

        #endregion

        #region ClearReflectionData Tests

        [Fact]
        public void ClearReflectionData_ValidCommand_ClearsAllReflectionTargets()
        {
            // Arrange
            var command = new CommandData();
            
            // Set some reflection targets
            command.ReflectionData.TargetIds[0] = 100;
            command.ReflectionData.TargetIds[1] = 200;
            command.ReflectionData.TargetIds[2] = 300;
            command.ReflectionData.TargetIds[3] = 400;

            // Act
            BattleCommandService.ClearReflectionData(command);

            // Assert
            for (int i = 0; i < command.ReflectionData.TargetIds.Length; i++)
            {
                Assert.Equal((UInt16)0, command.ReflectionData.TargetIds[i]);
            }
        }

        [Fact]
        public void ClearReflectionData_NullCommand_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => BattleCommandService.ClearReflectionData(null));
            Assert.Equal("command", exception.ParamName);
        }

        [Fact]
        public void ClearReflectionData_AlreadyCleanReflectionData_RemainsClean()
        {
            // Arrange
            var command = new CommandData();

            // Act
            BattleCommandService.ClearReflectionData(command);

            // Assert - should all be zero (default state)
            for (int i = 0; i < command.ReflectionData.TargetIds.Length; i++)
            {
                Assert.Equal((UInt16)0, command.ReflectionData.TargetIds[i]);
            }
        }

        #endregion

        #region ValidateCommand Tests

        [Fact]
        public void ValidateCommand_ValidCommand_ReturnsTrue()
        {
            // Arrange
            var command = new CommandData
            {
                CommandId = BattleCommandId.Attack,
                TargetId = 1
            };

            // Act
            var result = BattleCommandService.ValidateCommand(command);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateCommand_NullCommand_ReturnsFalse()
        {
            // Act
            var result = BattleCommandService.ValidateCommand(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCommand_NoCommandId_ReturnsFalse()
        {
            // Arrange
            var command = new CommandData
            {
                CommandId = BattleCommandId.None,
                TargetId = 1
            };

            // Act
            var result = BattleCommandService.ValidateCommand(command);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCommand_NoTargetId_ReturnsFalse()
        {
            // Arrange
            var command = new CommandData
            {
                CommandId = BattleCommandId.Attack,
                TargetId = 0
            };

            // Act
            var result = BattleCommandService.ValidateCommand(command);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CreateCommand Tests

        [Fact]
        public void CreateCommand_ReturnsNewValidCommand()
        {
            // Act
            var command = BattleCommandService.CreateCommand();

            // Assert
            Assert.NotNull(command);
            Assert.NotNull(command.Info);
            Assert.NotNull(command.ReflectionData);
            Assert.Equal(BattleCommandId.None, command.CommandId);
            Assert.Equal((UInt16)0, command.TargetId);
            Assert.Equal(4, command.ReflectionData.TargetIds.Length);
        }

        #endregion

        #region CopyCommand Tests

        [Fact]
        public void CopyCommand_ValidSource_CreatesExactCopy()
        {
            // Arrange
            var source = new CommandData
            {
                TargetId = 123,
                MagicCasterId = 456,
                CommandId = BattleCommandId.BlackMagic,
                SubNumber = 7,
                IsShortRange = true,
                HitRate = 85,
                Power = 150,
                ScriptId = 99
            };
            
            source.Info.Cursor = 2;
            source.Info.Status = 5;
            source.Info.Priority = 3;
            source.Info.CustomMPCost = 15;
            source.Info.ReflectNull = true;
            source.Info.CommandMotion = true;
            
            source.ReflectionData.TargetIds[0] = 111;
            source.ReflectionData.TargetIds[1] = 222;
            source.ReflectionData.TargetIds[2] = 333;
            source.ReflectionData.TargetIds[3] = 444;

            // Act
            var copy = BattleCommandService.CopyCommand(source);

            // Assert
            Assert.NotSame(source, copy);
            Assert.NotSame(source.Info, copy.Info);
            Assert.NotSame(source.ReflectionData, copy.ReflectionData);
            
            // Verify all properties match
            Assert.Equal(source.TargetId, copy.TargetId);
            Assert.Equal(source.MagicCasterId, copy.MagicCasterId);
            Assert.Equal(source.CommandId, copy.CommandId);
            Assert.Equal(source.SubNumber, copy.SubNumber);
            Assert.Equal(source.IsShortRange, copy.IsShortRange);
            Assert.Equal(source.HitRate, copy.HitRate);
            Assert.Equal(source.Power, copy.Power);
            Assert.Equal(source.ScriptId, copy.ScriptId);
            
            // Verify info properties match
            Assert.Equal(source.Info.Cursor, copy.Info.Cursor);
            Assert.Equal(source.Info.Status, copy.Info.Status);
            Assert.Equal(source.Info.Priority, copy.Info.Priority);
            Assert.Equal(source.Info.CustomMPCost, copy.Info.CustomMPCost);
            Assert.Equal(source.Info.ReflectNull, copy.Info.ReflectNull);
            Assert.Equal(source.Info.CommandMotion, copy.Info.CommandMotion);
            
            // Verify reflection data matches
            for (int i = 0; i < source.ReflectionData.TargetIds.Length; i++)
            {
                Assert.Equal(source.ReflectionData.TargetIds[i], copy.ReflectionData.TargetIds[i]);
            }
        }

        [Fact]
        public void CopyCommand_NullSource_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => BattleCommandService.CopyCommand(null));
            Assert.Equal("source", exception.ParamName);
        }

        [Fact]
        public void CopyCommand_ModifyingCopy_DoesNotAffectOriginal()
        {
            // Arrange
            var source = new CommandData
            {
                TargetId = 100,
                CommandId = BattleCommandId.Attack
            };
            source.Info.Cursor = 1;
            source.ReflectionData.TargetIds[0] = 50;

            // Act
            var copy = BattleCommandService.CopyCommand(source);
            copy.TargetId = 999;
            copy.CommandId = BattleCommandId.Steal;
            copy.Info.Cursor = 99;
            copy.ReflectionData.TargetIds[0] = 999;

            // Assert - original should be unchanged
            Assert.Equal((UInt16)100, source.TargetId);
            Assert.Equal(BattleCommandId.Attack, source.CommandId);
            Assert.Equal((Byte)1, source.Info.Cursor);
            Assert.Equal((UInt16)50, source.ReflectionData.TargetIds[0]);
        }

        #endregion
    }
}