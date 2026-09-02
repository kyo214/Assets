using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLoaderForRepoStreamingAssets : BGLoaderForRepo
{
	public const string LoaderName = "StreamingAssets";

	public const string FileName = "bansheegz_database.bytes";

	public const string FolderName = "StreamingAssets";

	private static MethodInfo assignDatabaseInstanceIdMethod;

	public override string Name => "StreamingAssets";

	public static string FilePath => GetFilePath("bansheegz_database.bytes");

	public static string AssetPath => Path.Combine(Path.Combine("Assets", "StreamingAssets"), "bansheegz_database.bytes");

	private static MethodInfo AssignDatabaseInstanceIdMethod
	{
		get
		{
			if (assignDatabaseInstanceIdMethod != null)
			{
				return assignDatabaseInstanceIdMethod;
			}
			Type type = BGUtil.GetType("BansheeGz.BGDatabase.Editor.BGEditorUtility");
			if (type == null)
			{
				return null;
			}
			assignDatabaseInstanceIdMethod = type.GetMethod("AssignDatabaseInstanceId", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(string) }, null);
			return assignDatabaseInstanceIdMethod;
		}
	}

	public override byte[] Load(LoadRequest request)
	{
		if (IsEmpty(request))
		{
			return Load("bansheegz_database.bytes", mainDatabase: true);
		}
		string fileName = ToPath(request);
		return Load(fileName, mainDatabase: false);
	}

	protected override string ToPath(LoadRequest request)
	{
		if (IsEmpty(request))
		{
			return "bansheegz_database.bytes";
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Path.GetFileName(request.basePath));
		string path = BGLoaderForRepo.AppendPaths(fileNameWithoutExtension, request.paths);
		return Path.ChangeExtension(path, "bytes");
	}

	private byte[] Load(string fileName, bool mainDatabase)
	{
		byte[] result = null;
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			result = LoadAndroid(fileName, mainDatabase);
			break;
		default:
			result = LoadDefault(fileName, mainDatabase);
			break;
		case RuntimePlatform.WebGLPlayer:
			break;
		}
		return result;
	}

	private byte[] LoadDefault(string fileName, bool mainDatabase)
	{
		string filePath = GetFilePath(fileName);
		if (!File.Exists(filePath))
		{
			return null;
		}
		if (mainDatabase)
		{
			BGRepo.DefaultRepoAssetPath = filePath;
			if (Application.isEditor)
			{
				TryToAssignInstanceId();
			}
		}
		return File.ReadAllBytes(filePath);
	}

	private void TryToAssignInstanceId()
	{
		BGRepo.DefaultRepoAssetId = 0;
		MethodInfo methodInfo = AssignDatabaseInstanceIdMethod;
		if (!(methodInfo == null))
		{
			methodInfo.Invoke(null, new object[1] { AssetPath });
		}
	}

	public static byte[] TestMeIfYouDare(string file)
	{
		return new BGLoaderForRepoStreamingAssets().TryToLoadAndroid(file, "bansheegz_database.bytes");
	}

	private byte[] LoadAndroid(string fileName, bool mainDatabase)
	{
		string dataPath = Application.dataPath;
		if (!File.Exists(dataPath))
		{
			return null;
		}
		try
		{
			byte[] array = TryToLoadAndroid(dataPath, fileName);
			if (array == null && !Application.isEditor && Path.GetFileName(dataPath) != "base.apk")
			{
				string text = Path.GetDirectoryName(dataPath) + "/base.apk";
				if (File.Exists(text))
				{
					array = TryToLoadAndroid(text, fileName);
				}
			}
			if ((array != null) & mainDatabase)
			{
				BGRepo.DefaultRepoAssetPath = fileName;
			}
			return array;
		}
		catch (Exception exception)
		{
			Debug.Log("Can not load BGDatabase from apk archive!");
			Debug.LogException(exception);
			return null;
		}
	}

	private byte[] TryToLoadAndroid(string apkPath, string fileName)
	{
		using (FileStream fileStream = File.OpenRead(apkPath))
		{
			using BinaryReader reader = new BinaryReader(fileStream);
			if (!fileStream.CanRead)
			{
				return null;
			}
			if (!fileStream.CanSeek)
			{
				return null;
			}
			ZipArchiveUtils.ReadEndOfCentralDirectory(fileStream, reader, out var _, out var centralDirectoryStart);
			string text = "assets/" + fileName;
			try
			{
				fileStream.Seek(centralDirectoryStart, SeekOrigin.Begin);
				ZipCentralDirectoryFileHeader header;
				while (ZipCentralDirectoryFileHeader.TryReadBlock(reader, out header))
				{
					long uncompressedSize = header.UncompressedSize;
					if (header.CompressedSize != uncompressedSize || !text.Equals(Encoding.UTF8.GetString(header.Filename)))
					{
						continue;
					}
					fileStream.Seek(header.RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
					if (!ZipLocalFileHeader.TrySkipBlock(reader))
					{
						throw new ZipArchiveException("Local file header corrupt");
					}
					if (uncompressedSize > int.MaxValue)
					{
						throw new IOException("invalid size " + 2147483647L);
					}
					int num = (int)uncompressedSize;
					int num2 = 0;
					byte[] array = new byte[num];
					while (num > 0)
					{
						int num3 = fileStream.Read(array, num2, num);
						if (num3 == 0)
						{
							throw new EndOfStreamException();
						}
						num2 += num3;
						num -= num3;
					}
					return array;
				}
			}
			catch (EndOfStreamException inner)
			{
				throw new ZipArchiveException("CentralDirectoryInvalid", inner);
			}
		}
		return null;
	}

	private static string GetFilePath(string fileName)
	{
		return Path.Combine(Application.streamingAssetsPath, fileName);
	}
}
