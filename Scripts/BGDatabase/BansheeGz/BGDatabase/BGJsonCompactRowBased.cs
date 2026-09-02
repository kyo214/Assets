using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

internal class BGJsonCompactRowBased : BGJsonCompactReader.JsonEntitiesReader, BGJsonCompactWriter.JsonEntitiesWriter
{
	private const string EntityId = "_id";

	public bool ThrowIfError;

	public BGJsonFormatEnum Format => BGJsonFormatEnum.CompactRowBased;

	public void OnRead(JSONObject metaObject, BGMetaEntity meta, Dictionary<string, JSONObject> jsonObjects)
	{
		BGJsonCompactReader.ReadArray(metaObject, "Entities", (JSONNode node) =>
		{
			BGEntity bGEntity = meta.NewEntity(new BGId(BGJsonCompactReader.Str(node, "_id")));
			foreach (KeyValuePair<string, JSONNode> item in node)
			{
				string key = item.Key;
				if (!(key == "_id"))
				{
					string value = item.Value.Value;
					try
					{
						meta.GetField(key).FromString(bGEntity.Index, value);
					}
					catch (Exception ex)
					{
						Debug.Log(BGUtil.Format("Can not fetch value for field=$, entity id=$. Field Value =$. Error=$", key, bGEntity.Id, value, ex.Message));
						Debug.LogException(ex);
						if (ThrowIfError)
						{
							throw;
						}
					}
				}
			}
		});
	}

	public void OnWrite(JSONObject jsonMeta, BGMetaEntity meta, Dictionary<string, JSONObject> jsonObjects)
	{
		JSONArray jsonEntities = new JSONArray();
		jsonMeta.Add("Entities", jsonEntities);
		meta.ForEachEntity((BGEntity entity) =>
		{
			JSONObject jsonEntity = new JSONObject();
			jsonEntities.Add(jsonEntity);
			jsonEntity.Add("_id", entity.Id.ToString());
			meta.ForEachField((BGField field) =>
			{
				jsonEntity.Add(field.Name, field.ToString(entity.Index));
			});
		});
	}
}
