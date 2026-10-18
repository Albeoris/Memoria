#nullable disable
using System;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationDiagnostic
    {
        public ModValidationDiagnostic(ModValidationDiagnosticSeverity severity, String message)
        {
            Severity = severity;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public ModValidationDiagnosticSeverity Severity { get; }
        public String Message { get; }
    }
}
