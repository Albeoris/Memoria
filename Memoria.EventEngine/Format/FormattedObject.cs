using System;

namespace FF8.ScriptFormatter
{
    public sealed class FormattedObject
    {
        // public Field.Info Field { get; }
        public String ObjectName { get; }
        public String Scripts { get; }

        public FormattedObject(/*Field.Info field, */String objectName, String scripts)
        {
            // Field = field;
            ObjectName = objectName;
            Scripts = scripts;
        }
    }
}