using System;

namespace Memoria
{
    public static class GameLoopManager
    {
        public static event Action Start;
        public static event Action Update;

        public static void RaiseStartEvent()
        {
            Log.Message("[GameLoopManager] RaiseStartEvent");
            Start?.Invoke();
        }

        public static void RaiseUpdateEvent()
        {
            Update?.Invoke();
        }
    }
}
