using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGJsonCompactWriter
{
	internal interface JsonEntitiesWriter
	{
		BGJsonFormatEnum Format { get; }

		void OnWrite(JSONObject metaObject, BGMetaEntity meta, Dictionary<string, JSONObject> jsonObjects);
	}

	internal string Write(BGRepo repo, JsonEntitiesWriter entitiesWriter, bool skipData = false, bool removeSensitive = false)
	{
		JSONObject jSONObject = new JSONObject();
		WriteRepo(repo, skipData, jSONObject, entitiesWriter, removeSensitive);
		return jSONObject.ToString(4);
	}

	private static void WriteRepo(BGRepo repo, bool skipData, JSONObject jsonRoot, JsonEntitiesWriter entitiesWriter, bool removeSensitive)
	{
		jsonRoot.Add("ProducedBy", "BGDatabase");
		jsonRoot.Add("DbVersion", "1.8.9");
		jsonRoot.Add("DbBuild", "2024.02.09");
		jsonRoot.Add("Format", (int)entitiesWriter.Format);
		JSONArray jsonAddons = new JSONArray();
		jsonRoot.Add("Addons", jsonAddons);
		repo.Addons.ForEachAddon((BGAddon addon) =>
		{
			JSONObject jSONObject = new JSONObject();
			jsonAddons.Add(jSONObject);
			jSONObject.Add("Type", addon.GetType().FullName);
			if (removeSensitive && typeof(BGAddonLiveUpdate) == addon.GetType())
			{
				jSONObject.Add("Config", "{\"content\" : \"[sensitive]\"}");
			}
			else
			{
				jSONObject.Add("Config", addon.ConfigToString());
			}
		});
		JSONArray jsonMetas = new JSONArray();
		jsonRoot.Add("Metas", jsonMetas);
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			JSONObject jSONObject = new JSONObject();
			jsonMetas.Add(jSONObject);
			jSONObject.Add("Singleton", meta.Singleton);
			jSONObject.Add("UniqueName", meta.UniqueName);
			jSONObject.Add("EmptyName", meta.EmptyName);
			jSONObject.Add("UserDefinedReadonly", meta.UserDefinedReadonly);
			jSONObject.Add("RowsCount", meta.CountEntities);
			WriteObjMeta(meta, jSONObject);
			Dictionary<string, JSONObject> name2Field = new Dictionary<string, JSONObject>(meta.CountFields);
			JSONArray jsonFields = new JSONArray();
			jSONObject.Add("Fields", jsonFields);
			meta.ForEachField((BGField field) =>
			{
				JSONObject jSONObject2 = new JSONObject();
				name2Field[field.Name] = jSONObject2;
				jsonFields.Add(jSONObject2);
				jSONObject2.Add("DefaultValue", field.DefaultValue);
				jSONObject2.Add("Required", field.Required);
				jSONObject2.Add("UserDefinedReadonly", field.UserDefinedReadonly);
				jSONObject2.Add("CustomEditor", field.CustomEditorTypeAsString);
				jSONObject2.Add("StringFormatter", field.CustomStringFormatterTypeAsString);
				WriteObjMeta(field, jSONObject2);
			});
			JSONArray jsonKeys = new JSONArray();
			jSONObject.Add("Keys", jsonKeys);
			meta.ForEachKey((BGKey key) =>
			{
				JSONObject jSONObject2 = new JSONObject();
				jsonKeys.Add(jSONObject2);
				jSONObject2.Add("Id", key.Id.ToString());
				jSONObject2.Add("Unique", key.IsUnique);
				jSONObject2.Add("Name", key.Name);
				jSONObject2.Add("Comment", key.Comment);
				jSONObject2.Add("ControllerType", key.ControllerType);
				JSONArray fieldIds = new JSONArray();
				jSONObject2.Add("FieldIds", fieldIds);
				key.ForEachField((BGField field) =>
				{
					fieldIds.Add(field.Id.ToString());
				});
			});
			JSONArray jsonIndexes = new JSONArray();
			jSONObject.Add("Indexes", jsonIndexes);
			meta.ForEachIndex((BGIndex index) =>
			{
				JSONObject jSONObject2 = new JSONObject();
				jsonIndexes.Add(jSONObject2);
				jSONObject2.Add("Id", index.Id.ToString());
				jSONObject2.Add("Name", index.Name);
				jSONObject2.Add("FieldId", index.Field.Id.ToString());
				jSONObject2.Add("Comment", index.Comment);
				jSONObject2.Add("ControllerType", index.ControllerType);
			});
			if (!skipData)
			{
				entitiesWriter.OnWrite(jSONObject, meta, name2Field);
			}
		});
		JSONArray jsonViews = new JSONArray();
		jsonRoot.Add("Views", jsonViews);
		repo.ForEachView((BGMetaView view) =>
		{
			JSONObject jSONObject = new JSONObject();
			jsonViews.Add(jSONObject);
			jSONObject.Add("Id", view.Id.ToString());
			jSONObject.Add("Name", view.Name);
			jSONObject.Add("Addon", view.Addon);
			jSONObject.Add("Comment", view.Comment);
			jSONObject.Add("ControllerType", view.ControllerType);
			jSONObject.Add("Config", view.ConfigToString());
			JSONObject jSONObject2 = new JSONObject();
			WriteRepo(view.DelegateMeta.Repo, skipData: true, jSONObject2, entitiesWriter, removeSensitive);
			jSONObject.Add("Repo", jSONObject2);
			view.Mappings.Trim();
			JSONArray jSONArray = new JSONArray();
			jSONObject.Add("MetaMappings", jSONArray);
			BGId[] includedMetas = view.Mappings.IncludedMetas;
			for (int i = 0; i < includedMetas.Length; i++)
			{
				BGId bGId = includedMetas[i];
				JSONObject jSONObject3 = new JSONObject();
				jSONArray.Add(jSONObject3);
				jSONObject3.Add("MetaId", bGId.ToString());
			}
		});
	}

	private static void WriteObjMeta(BGMetaObject repoObj, JSONObject jsonObj)
	{
		jsonObj.Add("Id", repoObj.Id.ToString());
		jsonObj.Add("Name", repoObj.Name);
		jsonObj.Add("Addon", repoObj.Addon);
		jsonObj.Add("IsSystem", repoObj.System);
		jsonObj.Add("Type", repoObj.GetType().FullName);
		jsonObj.Add("Config", repoObj.ConfigToString());
		jsonObj.Add("Comment", repoObj.Comment);
		jsonObj.Add("ControllerType", repoObj.ControllerType);
	}
}
