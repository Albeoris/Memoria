using System.Threading;
using System;

namespace Compression;

internal class DeflateStreamAsyncResult : IAsyncResult
{
    public byte[] buffer;
    public int offset;
    public int count;
    public bool isWrite;

    private object m_AsyncObject;
    private object m_AsyncState;
    private AsyncCallback m_AsyncCallback;
    private object m_Result;

    internal bool m_CompletedSynchronously;

    private int m_InvokedCallback;
    private int m_Completed;
    private object m_Event;

    public object AsyncState => m_AsyncState;

    public WaitHandle AsyncWaitHandle
    {
        get
        {
            int completed = m_Completed;
            if (m_Event == null)
                Interlocked.CompareExchange(ref m_Event, new ManualResetEvent(completed != 0), null);
            ManualResetEvent manualResetEvent = (ManualResetEvent)m_Event;
            if (completed == 0 && m_Completed != 0)
                manualResetEvent.Set();
            return manualResetEvent;
        }
    }

    public bool CompletedSynchronously => m_CompletedSynchronously;
    public bool IsCompleted => m_Completed != 0;

    internal object Result => m_Result;

    public DeflateStreamAsyncResult(object asyncObject, object asyncState, AsyncCallback asyncCallback, byte[] buffer, int offset, int count)
    {
        this.buffer = buffer;
        this.offset = offset;
        this.count = count;
        m_CompletedSynchronously = true;
        m_AsyncObject = asyncObject;
        m_AsyncState = asyncState;
        m_AsyncCallback = asyncCallback;
    }

    internal void Close()
    {
        if (m_Event != null)
            ((ManualResetEvent)m_Event).Close();
    }

    internal void InvokeCallback(bool completedSynchronously, object result)
    {
        Complete(completedSynchronously, result);
    }

    internal void InvokeCallback(object result)
    {
        Complete(result);
    }

    private void Complete(bool completedSynchronously, object result)
    {
        m_CompletedSynchronously = completedSynchronously;
        Complete(result);
    }

    private void Complete(object result)
    {
        m_Result = result;
        Interlocked.Increment(ref m_Completed);
        if (m_Event != null)
            ((ManualResetEvent)m_Event).Set();
        if (Interlocked.Increment(ref m_InvokedCallback) == 1 && m_AsyncCallback != null)
            m_AsyncCallback(this);
    }
}
