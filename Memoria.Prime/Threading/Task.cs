﻿using System;
using System.Threading;
using Object = System.Object;

namespace Memoria.Prime.Threading
{
    public abstract class Task<T> : Task
    {
        protected T _result;

        public T Result
        {
            get
            {
                Wait();
                return _result;
            }
        }
    }

    public abstract class Task
    {
        public static Task Run(Action action)
        {
            Task task = new ActionTask(action);
            task.Run();
            return task;
        }

        public static Task Run<TState>(Action<TState> action, TState state)
        {
            Task task = new ActionTask<TState>(action, state);
            task.Run();
            return task;
        }

        public static Task<T> Run<T>(Func<T> function)
        {
            Task<T> task = new FunctionTask<T>(function);
            task.Run();
            return task;
        }

        public static Task<T> Run<TState, T>(Func<TState, T> function, TState state)
        {
            Task<T> task = new FunctionTask<TState, T>(function, state);
            task.Run();
            return task;
        }

        public TaskState State { get; private set; }
        public Exception Exception { get; private set; }
        public Boolean IsCompleted => State > TaskState.Running;

        private readonly ManualResetEvent _completedEvent;
        private Thread _currentThread;

        protected Task()
        {
            _completedEvent = new ManualResetEvent(false);
        }

        protected abstract void Invoke();

        public void Run()
        {
            _completedEvent.Reset();
            State = TaskState.WaitingToRun;
            ThreadPool.QueueUserWorkItem(RunSafe);
        }

        public Boolean WaitSafe(Int32 millisecondsTimeout = -1, Boolean skipLogging = false)
        {
            if (IsCompleted)
                return true;

            if (millisecondsTimeout > -1)
                return _completedEvent.WaitOne(millisecondsTimeout);

            DateTime startTime = DateTime.UtcNow;
            for (Int32 i = 0; i < 6; i++)
            {
                if (_completedEvent.WaitOne(5 * 60 * 1000))
                    break;

                if (!skipLogging)
                    Log.Warning("Long running task was detected. Lifetime: {0}, Stack: {1}", DateTime.UtcNow - startTime, Environment.StackTrace);
            }

            return true;
        }

        public Boolean Wait(Int32 millisecondsTimeout = -1)
        {
            if (WaitSafe(millisecondsTimeout))
            {
                CheckFaulted();
                return true;
            }

            return false;
        }

        public void CheckFaulted()
        {
            if (State == TaskState.Faulted)
                throw new Exception("Asynchronous operation failed.", Exception);
        }

        private void RunSafe(Object state)
        {
            try
            {
                _currentThread = Thread.CurrentThread;
                GameLoopManager.Quit += OnApplicationQuitSafe;
                State = TaskState.Running;
                Invoke();
                State = TaskState.Success;
            }
            catch (OperationCanceledException ex)
            {
                State = TaskState.Canceled;
                Exception = ex;
            }
            catch (Exception ex)
            {
                State = TaskState.Faulted;
                Exception = ex;
            }
            finally
            {
                _completedEvent.Set();
                GameLoopManager.Quit -= OnApplicationQuitSafe;
                _currentThread = null;
            }
        }

        private void OnApplicationQuitSafe()
        {
            try
            {
                _currentThread.Abort();
                _currentThread.Join(2000);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to abort task.");
            }
        }
    }
}