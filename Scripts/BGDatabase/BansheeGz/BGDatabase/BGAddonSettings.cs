using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "Settings", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerSettings")]
public class BGAddonSettings : BGAddon
{
	public enum FormatEnum : byte
	{
		Binary = 0,
		Json = 1
	}

	[Serializable]
	private class Settings
	{
		public bool MultiThreadedLoading;

		public bool ZippedContent;

		public string EncryptorType;

		public string EncryptorConfig;

		public bool EncryptSaveLoadAddon;

		public FormatEnum Format;
	}

	private FormatEnum format;

	private bool multiThreadedLoading;

	private bool zippedContent;

	private string encryptorType;

	private string encryptorConfig;

	private bool encryptSaveLoadAddon;

	private BGEncryptor encryptor;

	public bool MultiThreadedLoading
	{
		get
		{
			return multiThreadedLoading;
		}
		set
		{
			if (multiThreadedLoading != value)
			{
				multiThreadedLoading = value;
				FireChange();
			}
		}
	}

	public bool ZippedContent
	{
		get
		{
			return zippedContent;
		}
		set
		{
			if (zippedContent != value)
			{
				zippedContent = value;
				FireChange();
			}
		}
	}

	public string EncryptorType
	{
		get
		{
			return encryptorType;
		}
		set
		{
			if (!(encryptorType == value))
			{
				if (!string.IsNullOrEmpty(value))
				{
					encryptor = GetEncryptor(value);
				}
				else
				{
					encryptor = null;
				}
				encryptorType = value;
				FireChange();
			}
		}
	}

	public string EncryptorConfig
	{
		get
		{
			return encryptorConfig;
		}
		set
		{
			if (!(encryptorConfig == value))
			{
				encryptorConfig = value;
				FireChange();
			}
		}
	}

	public bool EncryptSaveLoadAddon
	{
		get
		{
			return encryptSaveLoadAddon;
		}
		set
		{
			if (encryptSaveLoadAddon != value)
			{
				encryptSaveLoadAddon = value;
				FireChange();
			}
		}
	}

	public BGEncryptor Encryptor
	{
		get
		{
			if (string.IsNullOrEmpty(encryptorType))
			{
				return null;
			}
			if (encryptor != null)
			{
				return encryptor;
			}
			encryptor = GetEncryptor(encryptorType);
			return encryptor;
		}
	}

	public FormatEnum Format
	{
		get
		{
			return format;
		}
		set
		{
			if (format != value)
			{
				format = value;
				FireChange();
			}
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new Settings
		{
			MultiThreadedLoading = multiThreadedLoading,
			ZippedContent = zippedContent,
			EncryptorType = encryptorType,
			EncryptorConfig = encryptorConfig,
			EncryptSaveLoadAddon = encryptSaveLoadAddon,
			Format = format
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		multiThreadedLoading = settings.MultiThreadedLoading;
		zippedContent = settings.ZippedContent;
		encryptorType = settings.EncryptorType;
		encryptorConfig = settings.EncryptorConfig;
		encryptSaveLoadAddon = settings.EncryptSaveLoadAddon;
		format = settings.Format;
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(32);
		bGBinaryWriter.AddInt(3);
		bGBinaryWriter.AddBool(multiThreadedLoading);
		bGBinaryWriter.AddBool(zippedContent);
		bGBinaryWriter.AddString(encryptorType);
		bGBinaryWriter.AddString(encryptorConfig);
		bGBinaryWriter.AddBool(encryptSaveLoadAddon);
		bGBinaryWriter.AddByte((byte)format);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			multiThreadedLoading = bGBinaryReader.ReadBool();
			zippedContent = bGBinaryReader.ReadBool();
			break;
		case 2:
		case 3:
			multiThreadedLoading = bGBinaryReader.ReadBool();
			zippedContent = bGBinaryReader.ReadBool();
			encryptorType = bGBinaryReader.ReadString();
			encryptorConfig = bGBinaryReader.ReadString();
			encryptSaveLoadAddon = bGBinaryReader.ReadBool();
			if (num == 3)
			{
				format = (FormatEnum)bGBinaryReader.ReadByte();
			}
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		return new BGAddonSettings
		{
			Repo = repo,
			multiThreadedLoading = multiThreadedLoading,
			zippedContent = zippedContent,
			encryptorType = encryptorType,
			encryptorConfig = encryptorConfig,
			encryptSaveLoadAddon = encryptSaveLoadAddon,
			format = format
		};
	}

	private static BGEncryptor GetEncryptor(string type)
	{
		Type type2 = BGUtil.GetType(type);
		if (type2 == null)
		{
			throw new BGException("Can not find encryptor type $", type);
		}
		if (!typeof(BGEncryptor).IsAssignableFrom(type2))
		{
			throw new BGException("Encryptor type $ does not implement BGEncryptor interface ", type);
		}
		return (BGEncryptor)Activator.CreateInstance(type2);
	}

	public static FormatEnum GetFormat(BGRepo repo)
	{
		return repo.Addons.Get<BGAddonSettings>()?.format ?? FormatEnum.Binary;
	}
}
