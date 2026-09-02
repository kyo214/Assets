using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGJsonCompactReader
{
	internal interface JsonEntitiesReader
	{
		BGJsonFormatEnum Format { get; }

		void OnRead(JSONObject metaObject, BGMetaEntity meta, Dictionary<string, JSONObject> jsonObjects);
	}

	private readonly BGRepo repo = new BGRepo();

	public BGRepo Repo => repo;

	internal BGJsonCompactReader(string content, JsonEntitiesReader entitiesReader, bool skipData = false)
	{
		JSONObject jSONObject = (JSONObject)JSONNode.Parse(content);
		string text = Str(jSONObject, "Format");
		if (string.IsNullOrEmpty(text))
		{
			throw new Exception("Can not find required [Format] attribute in json content, probably json has the wrong format");
		}
		BGJsonFormatEnum bGJsonFormatEnum = (BGJsonFormatEnum)byte.Parse(text);
		if (bGJsonFormatEnum != entitiesReader.Format)
		{
			throw new Exception($"Can not parse json: formats mismatch! Expected format is [{entitiesReader.Format}], actual format is [{bGJsonFormatEnum}] ");
		}
		ReadArray(jSONObject, "Addons", (JSONNode node) =>
		{
			repo.Addons.Add(BGAddon.Create(node["Type"], node["Config"]));
		});
		Read(jSONObject, repo, entitiesReader, skipData);
	}

	private static void Read(JSONObject root, BGRepo repo, JsonEntitiesReader entitiesReader, bool skipData = false)
	{
		ReadArray(root, "Metas", (JSONNode metaNode) =>
		{
			BGMetaEntity meta = BGMetaEntity.Create(repo, Str(metaNode, "Type"), new BGId(Str(metaNode, "Id")), Str(metaNode, "Name"), Str(metaNode, "Config"), Bool(metaNode, "IsSystem"), Str(metaNode, "Addon"), Bool(metaNode, "UniqueName"), Bool(metaNode, "Singleton"), Bool(metaNode, "EmptyName"));
			meta.Comment = Str(metaNode, "Comment");
			meta.ControllerType = Str(metaNode, "ControllerType");
			meta.UserDefinedReadonly = Bool(metaNode, "UserDefinedReadonly");
			Dictionary<string, JSONObject> name2Field = new Dictionary<string, JSONObject>(meta.CountFields);
			ReadArray((JSONObject)metaNode, "Fields", (JSONNode fieldNode) =>
			{
				BGField bGField = BGField.Create(meta, Str(fieldNode, "Type"), new BGId(Str(fieldNode, "Id")), Str(fieldNode, "Name"), Str(fieldNode, "Config"), Bool(fieldNode, "IsSystem"), Str(fieldNode, "Addon"), Str(fieldNode, "DefaultValue"), Bool(fieldNode, "Required"));
				bGField.CustomStringFormatterTypeAsString = Str(fieldNode, "StringFormatter");
				bGField.CustomEditorTypeAsString = Str(fieldNode, "CustomEditor");
				bGField.Comment = Str(fieldNode, "Comment");
				bGField.ControllerType = Str(fieldNode, "ControllerType");
				bGField.UserDefinedReadonly = Bool(fieldNode, "UserDefinedReadonly");
				name2Field[bGField.Name] = (JSONObject)fieldNode;
			});
			ReadArray((JSONObject)metaNode, "Keys", (JSONNode keyNode) =>
			{
				string keyName = Str(keyNode, "Name");
				List<BGField> fields = new List<BGField>();
				ReadArray((JSONObject)keyNode, "FieldIds", (JSONNode keyFieldNode) =>
				{
					BGId bGId = new BGId(keyFieldNode.Value);
					BGField field = meta.GetField(bGId, errorIfNotFound: false);
					if (field == null)
					{
						string text = keyName;
						BGId bGId2 = bGId;
						Debug.Log("Can not read key [" + text + "], can not find a field with id=" + bGId2.ToString());
					}
					else
					{
						fields.Add(field);
					}
				});
				if (fields.Count != 0)
				{
					BGKey bGKey = BGKey.Create(new BGId(Str(keyNode, "Id")), keyName, Bool(keyNode, "Unique"), fields.ToArray());
					bGKey.Comment = Str(keyNode, "Comment");
					bGKey.ControllerType = Str(keyNode, "ControllerType");
				}
			});
			ReadArray((JSONObject)metaNode, "Indexes", (JSONNode indexNode) =>
			{
				BGId id = new BGId(Str(indexNode, "Id"));
				BGId bGId = new BGId(Str(indexNode, "FieldId"));
				string text = Str(indexNode, "Name");
				BGField field = meta.GetField(bGId, errorIfNotFound: false);
				if (field == null)
				{
					BGId bGId2 = bGId;
					Debug.Log("Can not read index [" + text + "], can not find a field with id=" + bGId2.ToString());
				}
				else
				{
					BGIndex bGIndex = BGIndex.Create(id, text, field);
					bGIndex.Comment = Str(indexNode, "Comment");
					bGIndex.ControllerType = Str(indexNode, "ControllerType");
				}
			});
			if (!skipData)
			{
				entitiesReader.OnRead((JSONObject)metaNode, meta, name2Field);
			}
		});
		ReadArray(root, "Views", (JSONNode viewNode) =>
		{
			BGMetaView view = BGMetaView.Create(repo, new BGId(Str(viewNode, "Id")), Str(viewNode, "Name"));
			view.System = Bool(viewNode, "IsSystem");
			view.Addon = Str(viewNode, "Addon");
			view.Comment = Str(viewNode, "Comment");
			view.ControllerType = Str(viewNode, "ControllerType");
			view.ConfigFromString(Str(viewNode, "Config"));
			BGRepo bGRepo = new BGRepo();
			Read((JSONObject)viewNode["Repo"], bGRepo, entitiesReader, skipData: true);
			view.DelegateMeta = (BGMetaRow)bGRepo.GetMeta(view.Id);
			ReadArray((JSONObject)viewNode, "MetaMappings", (JSONNode node) =>
			{
				view.Mappings.Add(new BGId(Str(node, "MetaId")));
			});
		});
	}

	internal static string Str(JSONNode node, string name)
	{
		JSONNode jSONNode = node[name];
		if (!(jSONNode == null))
		{
			return jSONNode.Value;
		}
		return null;
	}

	internal static bool Bool(JSONNode node, string name)
	{
		JSONNode jSONNode = node[name];
		if (jSONNode != null)
		{
			return jSONNode.AsBool;
		}
		return false;
	}

	internal static int ReadArray(JSONObject node, string name, Action<JSONNode> action)
	{
		JSONNode jSONNode = node[name];
		if (!(jSONNode is JSONArray { Values: var values }))
		{
			return 0;
		}
		int num = 0;
		foreach (JSONNode item in values)
		{
			num++;
			action(item);
		}
		return num;
	}
}
