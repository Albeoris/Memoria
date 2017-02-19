using System;
using System.Threading;

namespace Unity.IO.Compression
{
	internal class DeflateStreamAsyncResult : IAsyncResult
	{
		public DeflateStreamAsyncResult(Object asyncObject, Object asyncState, AsyncCallback asyncCallback, Byte[] buffer, Int32 offset, Int32 count)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.count = count;
			this.m_CompletedSynchronously = true;
			this.m_AsyncObject = asyncObject;
			this.m_AsyncState = asyncState;
			this.m_AsyncCallback = asyncCallback;
		}

		public Object AsyncState
		{
			get
			{
				return this.m_AsyncState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				Int32 completed = this.m_Completed;
				if (this.m_Event == null)
				{
					Interlocked.CompareExchange(ref this.m_Event, new ManualResetEvent(completed != 0), null);
				}
				ManualResetEvent manualResetEvent = (ManualResetEvent)this.m_Event;
				if (completed == 0 && this.m_Completed != 0)
				{
					manualResetEvent.Set();
				}
				return manualResetEvent;
			}
		}

		public Boolean CompletedSynchronously
		{
			get
			{
				return this.m_CompletedSynchronously;
			}
		}

		public Boolean IsCompleted
		{
			get
			{
				return this.m_Completed != 0;
			}
		}

		internal Object Result
		{
			get
			{
				return this.m_Result;
			}
		}

		internal void Close()
		{
			if (this.m_Event != null)
			{
				((ManualResetEvent)this.m_Event).Close();
			}
		}

		internal void InvokeCallback(Boolean completedSynchronously, Object result)
		{
			this.Complete(completedSynchronously, result);
		}

		internal void InvokeCallback(Object result)
		{
			this.Complete(result);
		}

		private void Complete(Boolean completedSynchronously, Object result)
		{
			this.m_CompletedSynchronously = completedSynchronously;
			this.Complete(result);
		}

		private void Complete(Object result)
		{
			this.m_Result = result;
			Interlocked.Increment(ref this.m_Completed);
			if (this.m_Event != null)
			{
				((ManualResetEvent)this.m_Event).Set();
			}
			if (Interlocked.Increment(ref this.m_InvokedCallback) == 1 && this.m_AsyncCallback != null)
			{
				this.m_AsyncCallback(this);
			}
		}

		public Byte[] buffer;

		public Int32 offset;

		public Int32 count;

		public Boolean isWrite;

		private Object m_AsyncObject;

		private Object m_AsyncState;

		private AsyncCallback m_AsyncCallback;

		private Object m_Result;

		internal Boolean m_CompletedSynchronously;

		private Int32 m_InvokedCallback;

		private Int32 m_Completed;

		private Object m_Event;
	}
}
