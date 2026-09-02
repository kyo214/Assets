using System;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGRepoBinary : BGRepo.RepoReaderI, BGRepo.RepoWriterI
{
	public const int LastVersion = 8;

	public BGRepo Read(byte[] dataBytes)
	{
		if (dataBytes == null || dataBytes.Length < 5)
		{
			return new BGRepo();
		}
		int num = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(dataBytes, 0, 4));
		if (num < 1 || num > 8)
		{
			return FromJson(dataBytes, num);
		}
		return GetReader(num).Read(dataBytes);
	}

	public byte[] Write(BGRepo repo)
	{
		BGAddonSettings bGAddonSettings = repo.Addons.Get<BGAddonSettings>();
		if (bGAddonSettings != null && bGAddonSettings.Format == BGAddonSettings.FormatEnum.Json)
		{
			return ToJson(repo);
		}
		return GetWriter(8).Write(repo);
	}

	private BGRepo.RepoReaderI GetReader(int version)
	{
		return version switch
		{
			1 => new BGRepoBinaryV1(), 
			2 => new BGRepoBinaryV2(), 
			3 => new BGRepoBinaryV3(), 
			4 => new BGRepoBinaryV4(), 
			5 => new BGRepoBinaryV5(), 
			6 => new BGRepoBinaryV6(), 
			7 => new BGRepoBinaryV7(), 
			8 => new BGRepoBinaryV8(), 
			_ => throw new BGException("Invalid DB version: can not find a reader for database version $", version), 
		};
	}

	private BGRepo.RepoWriterI GetWriter(int version)
	{
		return GetReader(version) as BGRepo.RepoWriterI;
	}

	private static byte[] ToJson(BGRepo repo)
	{
		string s = BGJson.ExportToString(repo, skipData: false, removeSensitive: false, BGJsonFormatEnum.CompactRowBased);
		return Encoding.UTF8.GetBytes(s);
	}

	private static BGRepo FromJson(byte[] data, int version)
	{
		string text = Encoding.UTF8.GetString(data);
		if (text[0] != '{')
		{
			throw new Exception($"Can not read database: 1) unsupported version {version} for binary serialization\n" + "2) illegal json content, the first char is not {");
		}
		if (text.IndexOf("ProducedBy", 0, 100, StringComparison.Ordinal) == -1)
		{
			throw new Exception("Can not read database: content seems to be in json format, but required ProducedBy attribute is not found");
		}
		return new BGJsonCompactReader(text, new BGJsonCompactRowBased
		{
			ThrowIfError = true
		}).Repo;
	}
}
