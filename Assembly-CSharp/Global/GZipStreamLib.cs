using System;
using System.Collections;
using System.IO;
using Unity.IO.Compression;
using UnityEngine;

public class GZipStreamLib : MonoBehaviour
{
	private void Awake()
	{
		GZipStreamLib.Instance = this;
	}

	public Boolean IsGZipHeader(String filePath)
	{
		Boolean result = false;
		using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				if (fileStream.Length >= 2L)
				{
					Byte[] array = new Byte[2];
					binaryReader.Read(array, 0, 2);
					if (array[0] == 31 && array[1] == 139)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public Int64 GetOriginalFileLength(String filePath)
	{
		Int64 result = -1L;
		using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				if (fileStream.Length >= 4L)
				{
					fileStream.Seek(-4L, SeekOrigin.End);
					Byte[] array = new Byte[4];
					binaryReader.Read(array, 0, 4);
					result = (Int64)BitConverter.ToInt32(array, 0);
				}
			}
		}
		return result;
	}

	public Int64 GetCompressedFileLength(String filePath)
	{
		Int64 result = -1L;
		using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
		{
			result = fileStream.Length;
		}
		return result;
	}

	public Int64 GetDecompressedFileLength()
	{
		if (this.dstStream == null)
		{
			return -1L;
		}
		if (!this.dstStream.CanWrite)
		{
			return -1L;
		}
		return this.dstStream.Length;
	}

	public void DecompressFile(String srcFilePath, String dstFilePath, Int32 bufferSize, GZipStreamLib.OnDecompressFileFinishDelegate onFinishDelegate)
	{
		this.srcFilePath = srcFilePath;
		this.dstFilePath = dstFilePath;
		this.onDecompressFinishDelegate = onFinishDelegate;
		Byte[] buffer = new Byte[bufferSize];
		try
		{
			this.OpenStreams(srcFilePath, dstFilePath);
			base.StartCoroutine(this.DecompressFileInCoroutine(buffer, this.srcStream, this.srcGzStream, this.dstStream, this.dstWriter));
		}
		catch (Exception message)
		{
			global::Debug.LogError(message);
			this.CloseStreams();
			this.onDecompressFinishDelegate(GZipStreamLib.ErrorCode.Failure);
		}
	}

	private IEnumerator DecompressFileInCoroutine(Byte[] buffer, FileStream srcStream, GZipStream srcGzStream, FileStream dstStream, BinaryWriter dstWriter)
	{
		Int32 offset = 0;
		Int32 totalCount = 0;
		for (;;)
		{
			try
			{
				Int32 bytesRead = srcGzStream.Read(buffer, 0, (Int32)buffer.Length);
				if (bytesRead == 0)
				{
					break;
				}
				offset += bytesRead;
				totalCount += bytesRead;
				dstWriter.Write(buffer, 0, bytesRead);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				global::Debug.LogError(ex);
				this.CloseStreams();
				this.onDecompressFinishDelegate(GZipStreamLib.ErrorCode.Failure);
				yield break;
			}
			yield return 0;
		}
		global::Debug.Log("Total count: " + totalCount);
		this.CloseStreams();
		this.onDecompressFinishDelegate(GZipStreamLib.ErrorCode.Success);
		yield break;
	}

	public void CloseAllOpenedStreams()
	{
		this.CloseStreams();
	}

	private void OpenStreams(String srcFilePath, String dstFilePath)
	{
		this.srcStream = File.Open(srcFilePath, FileMode.Open, FileAccess.Read);
		this.srcGzStream = new GZipStream(this.srcStream, CompressionMode.Decompress, false);
		this.dstStream = File.Open(dstFilePath, FileMode.Create, FileAccess.Write);
		this.dstWriter = new BinaryWriter(this.dstStream);
	}

	private void CloseStreams()
	{
		if (this.dstWriter != null)
		{
			this.dstWriter.Close();
		}
		if (this.dstStream != null)
		{
			this.dstStream.Close();
		}
		if (this.srcGzStream != null)
		{
			this.srcGzStream.Close();
		}
		if (this.srcStream != null)
		{
			this.srcStream.Close();
		}
	}

	private GZipStreamLib.OnDecompressFileFinishDelegate onDecompressFinishDelegate;

	private String srcFilePath;

	private String dstFilePath;

	private FileStream srcStream;

	private GZipStream srcGzStream;

	private FileStream dstStream;

	private BinaryWriter dstWriter;

	public static GZipStreamLib Instance;

	public enum ErrorCode
	{
		Success,
		Failure
	}

	public delegate void OnDecompressFileFinishDelegate(GZipStreamLib.ErrorCode errCode);
}
