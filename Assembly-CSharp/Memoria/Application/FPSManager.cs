using System;
using UnityEngine;

namespace Memoria
{
    static class FPSManager
    {
        public delegate void UpdaterDelegate(Single smoothFactor);

        public static Int32 MainLoopUpdateCount
        {
            get
            {
                if (FPSManager._currentTick == Time.frameCount)
                    return FPSManager._currentLoopCount;
                AdvanceUpdateCounter();
                return FPSManager._currentLoopCount;
            }
        }

        public static UInt32 DelayedInputs => FPSManager._delayedInputs;
        public static UInt32 DelayedInputTriggers => FPSManager._delayedInputsOn;
        public static UInt32 DelayedInputReleases => FPSManager._delayedInputsOff;

        public static Boolean IsDelayedInputTrigger(UInt32 eventInputCode)
        {
            return (FPSManager._delayedInputsOn & eventInputCode) != 0;
        }

        // A smooth effect runs during frames at which the main loop doesn't run, for smoothing visual effects without updating the states of objects
        public static void AddSmoothEffect(UpdaterDelegate eff)
        {
            FPSManager._activeSmooth = eff;
        }

        public static void DelayMainLoop(Single timeSkip)
        {
            FPSManager._nextLoopTarget += timeSkip;
        }

        public static void SetTargetFPS(Int32 fps)
        {
            if ((QualitySettings.vSyncCount == 1) != Configuration.Graphics.VSync)
                QualitySettings.vSyncCount = Configuration.Graphics.VSync ? 1 : 0;

            if (fps != Application.targetFrameRate)
                Application.targetFrameRate = fps;
        }

        public static Int32 GetTargetFPS()
        {
            return Application.targetFrameRate;
        }

        public static Int32 GetEstimatedFps()
        {
            return (Int32)Mathf.Round(0.2f / Time.smoothDeltaTime) * 5;
        }

        public static void SetMainLoopSpeed(Int32 lps)
        {
            if (lps == FPSManager._loopPerSecond)
                return;
            if (FPSManager._currentTick != Time.frameCount)
                AdvanceUpdateCounter();
            Single oldLoopTarget = FPSManager._loopTarget;
            FPSManager._loopPerSecond = lps;
            FPSManager._loopTarget = 1f / lps;
            FPSManager._nextLoopTarget += FPSManager._loopTarget - oldLoopTarget;
        }

        public static Int32 GetMainLoopSpeed()
        {
            return FPSManager._loopPerSecond;
        }

        private static void AdvanceUpdateCounter()
        {
            if (FPSManager._currentTick + 1 != Time.frameCount)
            {
                FlushDelayedInputs();
                FPSManager._currentTick = Time.frameCount;
                FPSManager._nextLoopTarget = FPSManager._loopTarget / FF9StateSystem.Settings.FastForwardFactor;
                FPSManager._currentLoopCount = 1;
                FPSManager._smoothUpdateFactor = 0f;
                FlushSmoothUpdaters();
                return;
            }
            if (FPSManager._currentLoopCount > 0)
                FlushDelayedInputs();
            else
                CollectDelayedInputs();
            FPSManager._currentLoopCount = 0;
            FPSManager._nextLoopTarget -= Time.deltaTime;
            while (2 * FF9StateSystem.Settings.FastForwardFactor * FPSManager._nextLoopTarget < FPSManager._loopTarget)
            {
                FPSManager._nextLoopTarget += FPSManager._loopTarget / FF9StateSystem.Settings.FastForwardFactor;
                FPSManager._currentLoopCount++;
            }
            FPSManager._currentTick = Time.frameCount;
            if (FPSManager._currentLoopCount > 0)
            {
                FPSManager._smoothUpdateFactor = 0f;
                FlushSmoothUpdaters();
            }
            else
            {
                FPSManager._smoothUpdateFactor += Time.deltaTime * FPSManager._loopPerSecond;
                RunSmoothUpdaters();
            }
        }

        private static void RunSmoothUpdaters()
        {
            if (FPSManager._activeSmooth != null)
                FPSManager._activeSmooth(FPSManager._smoothUpdateFactor);
        }

        private static void FlushSmoothUpdaters()
        {
            FPSManager._activeSmooth = null;
        }

        private static void CollectDelayedInputs()
        {
            UInt32 inputs = EventInput.ReadInputLight();
            FPSManager._delayedInputs |= inputs & ~FPSManager._delayedInputsOff;
            FPSManager._delayedInputsOn |= inputs & ~FPSManager._delayedInputsPrevious;
            FPSManager._delayedInputsOff |= ~inputs & FPSManager._delayedInputsPrevious & ~FPSManager._delayedInputsOn;
        }

        private static void FlushDelayedInputs()
        {
            UInt32 inputs = EventInput.ReadInputLight();
            FPSManager._delayedInputs = inputs;
            FPSManager._delayedInputsPrevious = (FPSManager._delayedInputsOn | (FPSManager._delayedInputsPrevious & inputs)) & ~FPSManager._delayedInputsOff;
            FPSManager._delayedInputsOn = inputs & ~FPSManager._delayedInputsPrevious;
            FPSManager._delayedInputsOff = ~inputs & FPSManager._delayedInputsPrevious & ~FPSManager._delayedInputsOn;
        }

        private static Int32 _loopPerSecond = 30;
        private static Single _loopTarget = 0.03333334f;
        private static Single _nextLoopTarget = 0f;
        private static Int32 _currentTick = 0;
        private static Int32 _currentLoopCount = 0;
        private static Single _smoothUpdateFactor = 0f;
        private static UpdaterDelegate _activeSmooth;
        private static UInt32 _delayedInputs = 0;
        private static UInt32 _delayedInputsOn = 0;
        private static UInt32 _delayedInputsOff = 0;
        private static UInt32 _delayedInputsPrevious = 0;
    }
}
