using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "viewRelationMultiple", Folder = "Relation", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerViewRelationMultiple")]
public class BGFieldViewRelationMultiple : BGFieldManyRelationsMultiple, BGFieldViewRelationI
{
	[Serializable]
	private struct JsonConfig
	{
		public string ViewId;
	}

	public new const ushort CodeType = 98;

	private BGMetaView view;

	private BGId viewId;

	public override ushort TypeCode => 98;

	public BGMetaView View
	{
		get
		{
			if (view != null)
			{
				return view;
			}
			view = base.Repo.GetView(viewId);
			return view;
		}
	}

	public BGId ViewId => viewId;

	public override List<BGMetaEntity> RelatedMetas => View.Metas;

	public override List<BGId> ToIds => new List<BGId>(View.Mappings.IncludedMetas);

	public BGFieldViewRelationMultiple(BGMetaEntity meta, string name, BGMetaView to)
		: base(meta, name, new List<BGMetaEntity> { meta })
	{
		if (to == null)
		{
			base.Meta.Unregister(this);
			throw new BGException("'To' view can not be null or empty");
		}
		view = to;
		viewId = to.Id;
	}

	internal BGFieldViewRelationMultiple(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldViewRelationMultiple(meta, id, name);
	}

	public override void RemoveRelatedMeta(BGMetaEntity metaEntity)
	{
		OnRemoveRelatedMeta(metaEntity);
	}

	public override void AddRelatedMeta(BGMetaEntity metaEntity)
	{
	}

	protected override void CheckMetaId(BGEntity entity)
	{
		if (!View.Mappings.IsIncluded(entity.MetaId))
		{
			throw new BGException("Can not assign entity [$] as related entity for field [$]: meta [$] is not included in view [$]!", entity.FullName, base.FullName, entity.MetaName, View.Name);
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			ViewId = viewId.ToString()
		});
	}

	public override void ConfigFromString(string config)
	{
		if (!string.IsNullOrEmpty(config))
		{
			JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
			view = null;
			BGId.TryParse(jsonConfig.ViewId, out viewId);
		}
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(20);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddId(viewId);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			view = null;
			viewId = bGBinaryReader.ReadId();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}
}
