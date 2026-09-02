using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityScriptableObject", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityScriptableObject")]
public class BGFieldUnityScriptableObject : BGFieldUnityAssetA<ScriptableObject>
{
	[Serializable]
	private class JsonConfigSo : JsonConfig
	{
		public string SOType;

		public bool AllowSubclasses;
	}

	public const ushort CodeType = 54;

	private Type scriptableObjectType;

	private bool allowSubclasses;

	public override ushort TypeCode => 54;

	public Type ScriptableObjectType
	{
		get
		{
			return scriptableObjectType;
		}
		set
		{
			if (!(scriptableObjectType == value))
			{
				if (value != null && !value.IsSubclassOf(typeof(ScriptableObject)))
				{
					throw new BGException("scriptableObjectType should be a subclass of ScriptableObject type!");
				}
				scriptableObjectType = value;
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public bool AllowSubclasses
	{
		get
		{
			return allowSubclasses;
		}
		set
		{
			if (allowSubclasses != value)
			{
				allowSubclasses = value;
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public BGFieldUnityScriptableObject(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityScriptableObject(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityScriptableObject(meta, id, name);
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfigSo
		{
			LoaderType = assetLoader.GetType().FullName,
			LoaderConfig = assetLoader.ConfigToString(),
			SOType = scriptableObjectType?.FullName,
			AllowSubclasses = allowSubclasses
		});
	}

	public override void ConfigFromString(string config)
	{
		if (string.IsNullOrEmpty(config))
		{
			assetLoader = new BGAssetLoaderResources();
			return;
		}
		JsonConfigSo jsonConfigSo = JsonUtility.FromJson<JsonConfigSo>(config);
		assetLoader = BGUtil.Create<BGAssetLoaderA>(jsonConfigSo.LoaderType, includePrivateConstructors: false, Array.Empty<object>());
		assetLoader.ConfigFromString(jsonConfigSo.LoaderConfig);
		string sOType = jsonConfigSo.SOType;
		if (!string.IsNullOrEmpty(sOType))
		{
			scriptableObjectType = BGUtil.GetType(sOType);
		}
		allowSubclasses = jsonConfigSo.AllowSubclasses;
	}

	protected override void ConfigToBytes(BGBinaryWriter writer)
	{
		writer.AddString(scriptableObjectType?.AssemblyQualifiedName);
		writer.AddBool(allowSubclasses);
	}

	protected override void ConfigFromBytes(int version, BGBinaryReader reader)
	{
		if (version == 1)
		{
			string text = reader.ReadString();
			if (!string.IsNullOrEmpty(text))
			{
				scriptableObjectType = BGUtil.GetType(text);
			}
			allowSubclasses = reader.ReadBool();
			return;
		}
		throw new BGException("Unknown version: $", version);
	}
}
