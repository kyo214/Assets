using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "programmableObject", Folder = "Programmable", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCodedObject")]
public class BGFieldCodedObject : BGFieldCodedA<object>
{
	[Serializable]
	private class JsonConfigObject : JsonConfig
	{
		public string ObjectType;
	}

	public const ushort CodeType = 104;

	private string objectType;

	public override ushort TypeCode => 104;

	public string ObjectType
	{
		get
		{
			return objectType;
		}
		set
		{
			if (!(objectType == value))
			{
				objectType = value;
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public BGFieldCodedObject(BGMetaEntity meta, string name, Type delegateType)
		: base(meta, name, delegateType)
	{
	}

	protected internal BGFieldCodedObject(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCodedObject(meta, id, name);
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfigObject
		{
			DelegateClass = delegateClass,
			ObjectType = objectType
		});
	}

	public override void ConfigFromString(string config)
	{
		if (!string.IsNullOrEmpty(config))
		{
			JsonConfigObject jsonConfigObject = JsonUtility.FromJson<JsonConfigObject>(config);
			delegateClass = jsonConfigObject.DelegateClass;
			objectType = jsonConfigObject.ObjectType;
		}
	}

	protected override void ConfigToBytes(BGBinaryWriter writer)
	{
		writer.AddString(objectType);
	}

	protected override void ConfigFromBytes(int version, BGBinaryReader reader)
	{
		if (version == 1)
		{
			objectType = reader.ReadString();
			return;
		}
		throw new BGException("Unknown version: $", version);
	}
}
