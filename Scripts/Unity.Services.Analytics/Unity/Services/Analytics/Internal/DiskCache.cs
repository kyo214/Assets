using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unity.Services.Analytics.Internal;

internal class DiskCache : IDiskCache
{
	internal const string k_FileHeaderString = "UGSEventCache";

	internal const int k_CacheFileVersionOne = 1;

	internal const int k_CacheFileVersionTwo = 2;

	private readonly string k_CacheFilePath;

	private readonly IFileSystemCalls k_SystemCalls;

	private readonly long k_CacheFileMaximumSize;

	internal DiskCache(IFileSystemCalls systemCalls)
	{
		if (systemCalls.CanAccessFileSystem())
		{
			k_CacheFilePath = Application.persistentDataPath + "/eventcache";
		}
		k_SystemCalls = systemCalls;
		k_CacheFileMaximumSize = 5242880L;
	}

	internal DiskCache(string cacheFilePath, IFileSystemCalls systemCalls, long maximumFileSize)
	{
		k_CacheFilePath = cacheFilePath;
		k_SystemCalls = systemCalls;
		k_CacheFileMaximumSize = maximumFileSize;
	}

	public void Write(List<EventSummary> eventSummaries, Stream payload)
	{
		if (eventSummaries.Count <= 0 || !k_SystemCalls.CanAccessFileSystem())
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < eventSummaries.Count; i++)
		{
			if (eventSummaries[i].EndIndex < k_CacheFileMaximumSize)
			{
				num = eventSummaries[i].EndIndex;
				num2 = i + 1;
			}
		}
		using Stream output = k_SystemCalls.OpenFileForWriting(k_CacheFilePath);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		binaryWriter.Write("UGSEventCache");
		binaryWriter.Write(2);
		binaryWriter.Write(num2);
		for (int j = 0; j < num2; j++)
		{
			binaryWriter.Write(eventSummaries[j].StartIndex);
			binaryWriter.Write(eventSummaries[j].EndIndex);
			binaryWriter.Write(eventSummaries[j].Id);
		}
		long position = payload.Position;
		payload.Position = 0L;
		for (int k = 0; k < num; k++)
		{
			binaryWriter.Write((byte)payload.ReadByte());
		}
		payload.Position = position;
	}

	public void Clear()
	{
		if (k_SystemCalls.CanAccessFileSystem() && k_SystemCalls.FileExists(k_CacheFilePath))
		{
			k_SystemCalls.DeleteFile(k_CacheFilePath);
		}
	}

	public bool Read(List<EventSummary> eventSummaries, Stream buffer)
	{
		if (k_SystemCalls.CanAccessFileSystem() && k_SystemCalls.FileExists(k_CacheFilePath))
		{
			using Stream input = k_SystemCalls.OpenFileForReading(k_CacheFilePath);
			using BinaryReader binaryReader = new BinaryReader(input);
			try
			{
				if (binaryReader.ReadString() == "UGSEventCache")
				{
					int num = binaryReader.ReadInt32();
					switch (num)
					{
					case 1:
						ReadVersionOneCacheFile(in eventSummaries, binaryReader, in buffer);
						return true;
					case 2:
						ReadVersionTwoCacheFile(in eventSummaries, binaryReader, in buffer);
						return true;
					default:
						Debug.LogWarning($"Unable to read event cache file: unknown file format version {num}");
						Clear();
						break;
					}
				}
				else
				{
					Debug.LogWarning("Unable to read event cache file: corrupt");
					Clear();
				}
			}
			catch (Exception)
			{
				Debug.LogWarning("Unable to read event cache file: corrupt");
				Clear();
			}
		}
		return false;
	}

	private void ReadVersionOneCacheFile(in List<EventSummary> eventEndIndices, BinaryReader reader, in Stream buffer)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int endIndex = reader.ReadInt32();
			eventEndIndices.Add(new EventSummary
			{
				StartIndex = ((i != 0) ? eventEndIndices[eventEndIndices.Count - 1].EndIndex : 0),
				EndIndex = endIndex,
				Id = $"loadedFromOldCache{i}"
			});
		}
		buffer.SetLength(0L);
		buffer.Position = 0L;
		reader.ReadBytes(14);
		reader.BaseStream.CopyTo(buffer);
	}

	private void ReadVersionTwoCacheFile(in List<EventSummary> eventSummaries, BinaryReader reader, in Stream buffer)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int startIndex = reader.ReadInt32();
			int endIndex = reader.ReadInt32();
			string id = reader.ReadString();
			eventSummaries.Add(new EventSummary
			{
				StartIndex = startIndex,
				EndIndex = endIndex,
				Id = id
			});
		}
		buffer.SetLength(0L);
		buffer.Position = 0L;
		reader.BaseStream.CopyTo(buffer);
	}
}
