using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

internal class BGJsonCompactFieldBased : BGJsonCompactReader.JsonEntitiesReader, BGJsonCompactWriter.JsonEntitiesWriter
{
	private const string entityIds = "EntityIds";

	private const string entityValues = "EntityValues";

	public BGJsonFormatEnum Format => BGJsonFormatEnum.CompactFieldBased;

	public void OnRead(JSONObject metaObject, BGMetaEntity meta, Dictionary<string, JSONObject> name2Field)
	{
		BGJsonCompactReader.ReadArray(metaObject, "EntityIds", (JSONNode node) =>
		{
			meta.NewEntity(new BGId(node.Value));
		});
		foreach (KeyValuePair<string, JSONObject> item in name2Field)
		{
			string fieldName = item.Key;
			JSONObject value = item.Value;
			BGField field = meta.GetField(fieldName);
			if (field is BGFieldNested)
			{
				continue;
			}
			int counter = 0;
			int num = BGJsonCompactReader.ReadArray(value, "EntityValues", (JSONNode node) =>
			{
				try
				{
					field.FromString(counter++, node.Value);
				}
				catch (Exception ex)
				{
					Debug.Log(BGUtil.Format("Can not fetch value for field=$. Field Value =$. Error=$", fieldName, node.Value, ex.Message));
					Debug.LogException(ex);
				}
			});
			if (num != meta.CountEntities)
			{
				throw new Exception("Values count mismatch: " + $"field {fieldName} has {num} values, but it should have {meta.CountEntities}");
			}
		}
		BGJsonCompactReader.ReadArray(metaObject, "Entities", (JSONNode node) =>
		{
			BGEntity bGEntity = meta.NewEntity(new BGId(BGJsonCompactReader.Str(node, "Id")));
			foreach (KeyValuePair<string, JSONNode> item2 in node)
			{
				string key = item2.Key;
				if (!(key == "Id"))
				{
					string value2 = item2.Value.Value;
					try
					{
						meta.GetField(key).FromString(bGEntity.Index, value2);
					}
					catch (Exception ex)
					{
						Debug.Log(BGUtil.Format("Can not fetch value for field=$, entity id=$. Field Value =$. Error=$", key, bGEntity.Id, value2, ex.Message));
						Debug.LogException(ex);
					}
				}
			}
		});
	}

	public void OnWrite(JSONObject jsonMeta, BGMetaEntity meta, Dictionary<string, JSONObject> name2Field)
	{
		JSONArray ids = new JSONArray();
		jsonMeta.Add("EntityIds", ids);
		meta.ForEachEntity((BGEntity entity) =>
		{
			ids.Add(entity.Id.ToString());
		});
		meta.ForEachField((BGField field) =>
		{
			JSONObject jSONObject = name2Field[field.Name];
			JSONArray jSONArray = new JSONArray();
			jSONObject.Add("EntityValues", jSONArray);
			for (int i = 0; i < meta.CountEntities; i++)
			{
				jSONArray.Add(field.ToString(i));
			}
		}, (BGField field) => !(field is BGFieldNested));
	}
}
