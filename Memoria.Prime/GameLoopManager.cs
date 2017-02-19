using System;

namespace Memoria.Prime
{
    public static class GameLoopManager
    {
        public static event Action Start;
        public static event Action Update;
        public static event Action Quit;

        public static void RaiseStartEvent()
        {
            Log.Message("[GameLoopManager] RaiseStartEvent");
            Start?.Invoke();
        }

        public static void RaiseUpdateEvent()
        {
            Update?.Invoke();
        }

        public static void RaiseQuitEvent()
        {
            Log.Message("[GameLoopManager] RaiseQuitEvent");
            Quit?.Invoke();
        }
    }
}
