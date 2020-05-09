using System;
using System.Collections;
using Memoria.Prime.Threading;

namespace Memoria.Assets
{
    public abstract class TextImporter
    {
        public IEnumerable LoadAsync()
        {
            Task<Boolean> task;
            if (Configuration.Import.Text)
            {
                task = Task.Run(LoadExternal);
                while (!task.IsCompleted)
                    yield return 1;

                if (task.State == TaskState.Success && task.Result)
                    yield break;
            }

            task = Task.Run(LoadInternal);
            while (!task.IsCompleted)
                yield return 1;

            if (!task.Result)
                throw new Exception("Failed to load embaded resources.");
        }

        protected abstract Boolean LoadExternal();
        protected abstract Boolean LoadInternal();
    }
}