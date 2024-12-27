using System.Security.Permissions;
using System.Threading;
using System.IO;
using System;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace Compression;

/// <summary>Provides methods and properties for compressing and decompressing streams using the Deflate algorithm.</summary>
public class DeflateStream : Stream
{
    internal delegate void AsyncWriteDelegate(byte[] array, int offset, int count, bool isAsync);

    private const int bufferSize = 4096;

    private Stream _stream;
    private CompressionMode _mode;
    private bool _leaveOpen;
    private Inflater inflater;
    private Deflater deflater;
    private byte[] buffer;
    private int asyncOperations;

    private readonly AsyncCallback m_CallBack;
    private readonly AsyncWriteDelegate m_AsyncWriterDelegate;

    /// <summary>Gets a value indicating whether the stream supports reading while decompressing a file.</summary>
    /// <returns>true if the System.IO.Compression.CompressionMode value is Decompress, and the underlying stream is opened and supports reading; otherwise, false.</returns>
    public override bool CanRead
    {
        get
        {
            if (_stream == null)
                return false;
            if (_mode == CompressionMode.Decompress)
                return _stream.CanRead;
            return false;
        }
    }

    /// <summary>Gets a value indicating whether the stream supports writing.</summary>
    /// <returns>true if the System.IO.Compression.CompressionMode value is Compress, and the underlying stream supports writing and is not closed; otherwise, false.</returns>
    public override bool CanWrite
    {
        get
        {
            if (_stream == null)
                return false;
            if (_mode == CompressionMode.Compress)
                return _stream.CanWrite;
            return false;
        }
    }

    /// <summary>Gets a value indicating whether the stream supports seeking.</summary>
    /// <returns>false in all cases.</returns>
    public override bool CanSeek => false;

    /// <summary>This property is not supported and always throws a System.NotSupportedException.</summary>
    /// <returns>A long value.</returns>
    /// <exception cref="NotSupportedException">This property is not supported on this stream.</exception>
    public override long Length => throw new NotSupportedException("NotSupported");

    /// <summary>This property is not supported and always throws a System.NotSupportedException.</summary>
    /// <returns>A long value.</returns>
    /// <exception cref="NotSupportedException">This property is not supported on this stream.</exception>
    public override long Position
    {
        get => throw new NotSupportedException("NotSupported");
        set => throw new NotSupportedException("NotSupported");
    }

    /// <summary>Gets a reference to the underlying stream.</summary>
    /// <returns>A stream object that represents the underlying stream.</returns>
    /// <exception cref="ObjectDisposedException">The underlying stream is closed.</exception>
    public Stream BaseStream => _stream;

    /// <summary>Initializes a new instance of the System.IO.Compression.DeflateStream class using the specified stream and System.IO.Compression.CompressionMode value.</summary>
    /// <param name="stream">The stream to compress or decompress.</param>
    /// <param name="mode">One of the System.IO.Compression.CompressionMode values that indicates the action to take.</param>
    /// <exception cref="ArgumentNullException">stream is null.</exception>
    /// <exception cref="InvalidOperationException">stream access right is ReadOnly and mode value is Compress.</exception>
    /// <exception cref="ArgumentException">mode is not a valid System.IO.Compression.CompressionMode value.
    /// -or-System.IO.Compression.CompressionMode is System.IO.Compression.CompressionMode.Compress and System.IO.Stream.CanWrite is false.
    /// -or-System.IO.Compression.CompressionMode is System.IO.Compression.CompressionMode.Decompress and System.IO.Stream.CanRead is false.</exception>
    public DeflateStream(Stream stream, CompressionMode mode)
        : this(stream, mode, leaveOpen: false, usingGZip: false)
    {
    }

    /// <summary>Initializes a new instance of the System.IO.Compression.DeflateStream class using the specified stream and System.IO.Compression.CompressionMode value, and a value that specifies whether to leave the stream open.</summary>
    /// <param name="stream">The stream to compress or decompress.</param>
    /// <param name="mode">One of the System.IO.Compression.CompressionMode values that indicates the action to take.</param>
    /// <param name="leaveOpen">true to leave the stream open; otherwise, false.</param>
    /// <exception cref="ArgumentNullException">stream is null.</exception>
    /// <exception cref="InvalidOperationException">stream access right is ReadOnly and mode value is Compress.</exception>
    /// <exception cref="ArgumentException">mode is not a valid System.IO.Compression.CompressionMode value.
    /// -or-System.IO.Compression.CompressionMode is System.IO.Compression.CompressionMode.Compress and System.IO.Stream.CanWrite is false.
    /// -or-System.IO.Compression.CompressionMode is System.IO.Compression.CompressionMode.Decompress and System.IO.Stream.CanRead is false.</exception>
    public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
        : this(stream, mode, leaveOpen, usingGZip: false)
    {
    }

    internal DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen, bool usingGZip)
    {
        _stream = stream;
        _mode = mode;
        _leaveOpen = leaveOpen;
        if (_stream == null)
            throw new ArgumentNullException("stream");
        switch (_mode)
        {
            case CompressionMode.Decompress:
                if (!_stream.CanRead)
                    throw new ArgumentException("NotReadableStream", "stream");
                inflater = new Inflater(usingGZip);
                m_CallBack = ReadCallback;
                break;
            case CompressionMode.Compress:
                if (!_stream.CanWrite)
                    throw new ArgumentException("NotWriteableStream", "stream");
                deflater = new Deflater(usingGZip);
                m_AsyncWriterDelegate = InternalWrite;
                m_CallBack = WriteCallback;
                break;
            default:
                throw new ArgumentException("ArgumentOutOfRange_Enum", "mode");
        }
        buffer = new byte[DeflateStream.bufferSize];
    }

    /// <summary>Flushes the contents of the internal buffer of the current stream object to the underlying stream.</summary>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    public override void Flush()
    {
        if (_stream == null)
            throw new ObjectDisposedException(null, "ObjectDisposed_StreamClosed");
    }

    /// <summary>This operation is not supported and always throws a System.NotSupportedException.</summary>
    /// <param name="offset">The location in the stream.</param>
    /// <param name="origin">One of the System.IO.SeekOrigin values.</param>
    /// <returns>A long value.</returns>
    /// <exception cref="NotSupportedException">This property is not supported on this stream.</exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException("NotSupported");
    }

    /// <summary>This operation is not supported and always throws a System.NotSupportedException.</summary>
    /// <param name="value">The length of the stream.</param>
    /// <exception cref="NotSupportedException">This property is not supported on this stream.</exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException("NotSupported");
    }

    /// <summary>Reads a specified number of compressed or decompressed bytes into the specified byte array.</summary>
    /// <param name="array">The buffer to store the compressed or decompressed bytes.</param>
    /// <param name="offset">The location in the array to begin reading.</param>
    /// <param name="count">The number of compressed or decompressed bytes to read.</param>
    /// <returns>The number of bytes that were read into the byte array.</returns>
    /// <exception cref="ArgumentNullException">array is null.</exception>
    /// <exception cref="InvalidOperationException">The System.IO.Compression.CompressionMode value was Compress when the object was created.
    /// -or-The underlying stream does not support reading.</exception>
    /// <exception cref="ArgumentOutOfRangeException">offset or count is less than zero.
    /// -or-array length minus the index starting point is less than count.</exception>
    /// <exception cref="InvalidDataException">The data is in an invalid format.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    public override int Read(byte[] array, int offset, int count)
    {
        EnsureDecompressionMode();
        ValidateParameters(array, offset, count);
        int currentOffset = offset;
        int remaining = count;
        while (true)
        {
            int readCount = inflater.Inflate(array, currentOffset, remaining);
            currentOffset += readCount;
            remaining -= readCount;
            if (remaining == 0 || inflater.Finished())
                break;
            int num4 = _stream.Read(buffer, 0, buffer.Length);
            if (num4 == 0)
                break;
            inflater.SetInput(buffer, 0, num4);
        }
        return count - remaining;
    }

    private void ValidateParameters(byte[] array, int offset, int count)
    {
        if (array == null)
            throw new ArgumentNullException("array");
        if (offset < 0)
            throw new ArgumentOutOfRangeException("offset");
        if (count < 0)
            throw new ArgumentOutOfRangeException("count");
        if (array.Length - offset < count)
            throw new ArgumentException("InvalidArgumentOffsetCount");
        if (_stream == null)
            throw new ObjectDisposedException(null, "ObjectDisposed_StreamClosed");
    }

    private void EnsureDecompressionMode()
    {
        if (_mode != 0)
            throw new InvalidOperationException("CannotReadFromDeflateStream");
    }

    private void EnsureCompressionMode()
    {
        if (_mode != CompressionMode.Compress)
            throw new InvalidOperationException("CannotWriteToDeflateStream");
    }

    /// <summary>Begins an asynchronous read operation.</summary>
    /// <param name="array">The byte array to read the data into.</param>
    /// <param name="offset">The byte offset in array at which to begin writing data read from the stream.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <param name="asyncCallback">An optional asynchronous callback, to be called when the read is complete.</param>
    /// <param name="asyncState">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
    /// <returns>An System.IAsyncResult object that represents the asynchronous read, which could still be pending.</returns>
    /// <exception cref="IOException">An asynchronous read past the end of the stream was attempted, or a disk error occurred.</exception>
    /// <exception cref="ArgumentException">One or more of the arguments is invalid.</exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    /// <exception cref="NotSupportedException">The current System.IO.Compression.DeflateStream implementation does not support the read operation.</exception>
    /// <exception cref="InvalidOperationException">This call cannot be completed.</exception>
    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
    public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
    {
        EnsureDecompressionMode();
        if (asyncOperations != 0)
            throw new InvalidOperationException("InvalidBeginCall");
        Interlocked.Increment(ref asyncOperations);
        try
        {
            ValidateParameters(array, offset, count);
            DeflateStreamAsyncResult deflateStreamAsyncResult = new DeflateStreamAsyncResult(this, asyncState, asyncCallback, array, offset, count);
            deflateStreamAsyncResult.isWrite = false;
            int num = inflater.Inflate(array, offset, count);
            if (num != 0)
            {
                deflateStreamAsyncResult.InvokeCallback(completedSynchronously: true, num);
                return deflateStreamAsyncResult;
            }
            if (inflater.Finished())
            {
                deflateStreamAsyncResult.InvokeCallback(completedSynchronously: true, 0);
                return deflateStreamAsyncResult;
            }
            _stream.BeginRead(buffer, 0, buffer.Length, m_CallBack, deflateStreamAsyncResult);
            deflateStreamAsyncResult.m_CompletedSynchronously &= deflateStreamAsyncResult.IsCompleted;
            return deflateStreamAsyncResult;
        }
        catch
        {
            Interlocked.Decrement(ref asyncOperations);
            throw;
        }
    }

    private void ReadCallback(IAsyncResult baseStreamResult)
    {
        DeflateStreamAsyncResult deflateStreamAsyncResult = (DeflateStreamAsyncResult)baseStreamResult.AsyncState;
        deflateStreamAsyncResult.m_CompletedSynchronously &= baseStreamResult.CompletedSynchronously;
        int num = 0;
        try
        {
            num = _stream.EndRead(baseStreamResult);
        }
        catch (Exception result)
        {
            deflateStreamAsyncResult.InvokeCallback(result);
            return;
        }
        if (num <= 0)
        {
            deflateStreamAsyncResult.InvokeCallback(0);
            return;
        }
        inflater.SetInput(buffer, 0, num);
        num = inflater.Inflate(deflateStreamAsyncResult.buffer, deflateStreamAsyncResult.offset, deflateStreamAsyncResult.count);
        if (num == 0 && !inflater.Finished())
            _stream.BeginRead(buffer, 0, buffer.Length, m_CallBack, deflateStreamAsyncResult);
        else
            deflateStreamAsyncResult.InvokeCallback(num);
    }

    /// <summary>Waits for the pending asynchronous read to complete.</summary>
    /// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
    /// <returns>The number of bytes read from the stream, between zero (0) and the number of bytes you requested. System.IO.Compression.DeflateStream returns zero (0) only at the end of the stream; otherwise, it blocks until at least one byte is available.</returns>
    /// <exception cref="ArgumentNullException">asyncResult is null.</exception>
    /// <exception cref="ArgumentException">asyncResult did not originate from a System.IO.Compression.DeflateStream.BeginRead method on the current stream.</exception>
    /// <exception cref="SystemException">An exception was thrown during a call to System.Threading.WaitHandle.WaitOne.</exception>
    /// <exception cref="InvalidOperationException">The end call is invalid because asynchronous read operations for this stream are not yet complete.</exception>
    /// <exception cref="InvalidOperationException">The stream is null.</exception>
    public override int EndRead(IAsyncResult asyncResult)
    {
        EnsureDecompressionMode();
        if (asyncOperations != 1)
            throw new InvalidOperationException("InvalidEndCall");
        if (asyncResult == null)
            throw new ArgumentNullException("asyncResult");
        if (_stream == null)
            throw new InvalidOperationException("ObjectDisposed_StreamClosed");
        if (!(asyncResult is DeflateStreamAsyncResult deflateStreamAsyncResult))
            throw new ArgumentNullException("asyncResult");
        try
        {
            if (!deflateStreamAsyncResult.IsCompleted)
                deflateStreamAsyncResult.AsyncWaitHandle.WaitOne();
        }
        finally
        {
            Interlocked.Decrement(ref asyncOperations);
            deflateStreamAsyncResult.Close();
        }
        if (deflateStreamAsyncResult.Result is Exception)
            throw (Exception)deflateStreamAsyncResult.Result;
        return (int)deflateStreamAsyncResult.Result;
    }

    /// <summary>Writes compressed or decompressed bytes to the underlying stream from the specified byte array.</summary>
    /// <param name="array">The buffer that contains the data to compress or decompress.</param>
    /// <param name="offset">The byte offset in array at which the read bytes will be placed.</param>
    /// <param name="count">The maximum number of bytes to compress.</param>
    public override void Write(byte[] array, int offset, int count)
    {
        EnsureCompressionMode();
        ValidateParameters(array, offset, count);
        InternalWrite(array, offset, count, isAsync: false);
    }

    internal void InternalWrite(byte[] array, int offset, int count, bool isAsync)
    {
        while (!deflater.NeedsInput())
        {
            int deflateOutput = deflater.GetDeflateOutput(buffer);
            if (deflateOutput != 0)
            {
                if (isAsync)
                {
                    IAsyncResult asyncResult = _stream.BeginWrite(buffer, 0, deflateOutput, null, null);
                    _stream.EndWrite(asyncResult);
                }
                else
                {
                    _stream.Write(buffer, 0, deflateOutput);
                }
            }
        }
        deflater.SetInput(array, offset, count);
        while (!deflater.NeedsInput())
        {
            int deflateOutput = deflater.GetDeflateOutput(buffer);
            if (deflateOutput != 0)
            {
                if (isAsync)
                {
                    IAsyncResult asyncResult2 = _stream.BeginWrite(buffer, 0, deflateOutput, null, null);
                    _stream.EndWrite(asyncResult2);
                }
                else
                {
                    _stream.Write(buffer, 0, deflateOutput);
                }
            }
        }
    }

    /// <summary>Releases the unmanaged resources used by the System.IO.Compression.DeflateStream and optionally releases the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (!disposing || _stream == null)
                return;
            Flush();
            if (_mode != CompressionMode.Compress || _stream == null)
                return;
            int deflateOutput;
            while (!deflater.NeedsInput())
            {
                deflateOutput = deflater.GetDeflateOutput(buffer);
                if (deflateOutput != 0)
                    _stream.Write(buffer, 0, deflateOutput);
            }
            deflateOutput = deflater.Finish(buffer);
            if (deflateOutput > 0)
                _stream.Write(buffer, 0, deflateOutput);
        }
        finally
        {
            try
            {
                if (disposing && !_leaveOpen && _stream != null)
                    _stream.Close();
            }
            finally
            {
                _stream = null;
                base.Dispose(disposing);
            }
        }
    }

    /// <summary>Begins an asynchronous write operation.</summary>
    /// <param name="array">The buffer to write data from.</param>
    /// <param name="offset">The byte offset in buffer to begin writing from.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    /// <param name="asyncCallback">An optional asynchronous callback, to be called when the write is complete.</param>
    /// <param name="asyncState">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
    /// <returns>An System.IAsyncResult object that represents the asynchronous write, which could still be pending.</returns>
    /// <exception cref="IOException">An asynchronous write past the end of the stream was attempted, or a disk error occurred.</exception>
    /// <exception cref="ArgumentException">One or more of the arguments is invalid.</exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    /// <exception cref="NotSupportedException">The current System.IO.Compression.DeflateStream implementation does not support the write operation.</exception>
    /// <exception cref="InvalidOperationException">The write operation cannot be performed because the stream is closed.</exception>
    [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
    public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
    {
        EnsureCompressionMode();
        if (asyncOperations != 0)
            throw new InvalidOperationException("InvalidBeginCall");
        Interlocked.Increment(ref asyncOperations);
        try
        {
            ValidateParameters(array, offset, count);
            DeflateStreamAsyncResult deflateStreamAsyncResult = new DeflateStreamAsyncResult(this, asyncState, asyncCallback, array, offset, count);
            deflateStreamAsyncResult.isWrite = true;
            m_AsyncWriterDelegate.BeginInvoke(array, offset, count, isAsync: true, m_CallBack, deflateStreamAsyncResult);
            deflateStreamAsyncResult.m_CompletedSynchronously &= deflateStreamAsyncResult.IsCompleted;
            return deflateStreamAsyncResult;
        }
        catch
        {
            Interlocked.Decrement(ref asyncOperations);
            throw;
        }
    }

    private void WriteCallback(IAsyncResult asyncResult)
    {
        DeflateStreamAsyncResult deflateStreamAsyncResult = (DeflateStreamAsyncResult)asyncResult.AsyncState;
        deflateStreamAsyncResult.m_CompletedSynchronously &= asyncResult.CompletedSynchronously;
        try
        {
            m_AsyncWriterDelegate.EndInvoke(asyncResult);
        }
        catch (Exception result)
        {
            deflateStreamAsyncResult.InvokeCallback(result);
            return;
        }
        deflateStreamAsyncResult.InvokeCallback(null);
    }

    /// <summary>Ends an asynchronous write operation.</summary>
    /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
    /// <exception cref="ArgumentNullException">asyncResult is null.</exception>
    /// <exception cref="ArgumentException">asyncResult did not originate from a System.IO.Compression.DeflateStream.BeginWrite method on the current stream.</exception>
    /// <exception cref="Exception">An exception was thrown during a call to System.Threading.WaitHandle.WaitOne.</exception>
    /// <exception cref="InvalidOperationException">The stream is null.</exception>
    /// <exception cref="InvalidOperationException">The end write call is invalid.</exception>
    public override void EndWrite(IAsyncResult asyncResult)
    {
        EnsureCompressionMode();
        if (asyncOperations != 1)
            throw new InvalidOperationException("InvalidEndCall");
        if (asyncResult == null)
            throw new ArgumentNullException("asyncResult");
        if (_stream == null)
            throw new InvalidOperationException("ObjectDisposed_StreamClosed");
        if (!(asyncResult is DeflateStreamAsyncResult deflateStreamAsyncResult))
            throw new ArgumentNullException("asyncResult");
        try
        {
            if (!deflateStreamAsyncResult.IsCompleted)
                deflateStreamAsyncResult.AsyncWaitHandle.WaitOne();
        }
        finally
        {
            Interlocked.Decrement(ref asyncOperations);
            deflateStreamAsyncResult.Close();
        }
        if (deflateStreamAsyncResult.Result is Exception)
            throw (Exception)deflateStreamAsyncResult.Result;
    }
}
