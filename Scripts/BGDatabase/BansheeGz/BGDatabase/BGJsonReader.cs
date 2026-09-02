using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGJsonReader
{
	private readonly BGRepo repo = new BGRepo();

	public BGRepo Repo => repo;

	public BGJsonReader(string content, bool skipData = false)
	{
		BGJsonRepoModel bGJsonRepoModel = JsonUtility.FromJson<BGJsonRepoModel>(content);
		BGUtil.ForEach(bGJsonRepoModel.Addons, (BGJsonRepoModel.Addon addon) =>
		{
			repo.Addons.Add(BGAddon.Create(addon.Type, addon.Config));
		});
		Read(bGJsonRepoModel, repo, skipData);
	}

	public static void Read(BGJsonRepoModel model, BGRepo repo, bool skipData = false)
	{
		if (model.Format != BGJsonFormatEnum.Classic)
		{
			throw new Exception($"Can not import JSON: json format mismatch, expected format is {BGJsonFormatEnum.Classic}," + $" the actual format is {model.Format}");
		}
		BGUtil.ForEach(model.Metas, (BGJsonRepoModel.Meta jsonMeta) =>
		{
			BGMetaEntity meta = BGMetaEntity.Create(repo, jsonMeta.Type, new BGId(jsonMeta.Id), jsonMeta.Name, jsonMeta.Config, jsonMeta.IsSystem, jsonMeta.Addon, jsonMeta.UniqueName, jsonMeta.Singleton, jsonMeta.EmptyName);
			meta.Comment = jsonMeta.Comment;
			meta.ControllerType = jsonMeta.ControllerType;
			meta.UserDefinedReadonly = jsonMeta.UserDefinedReadonly;
			BGUtil.ForEach(jsonMeta.Fields, (BGJsonRepoModel.Field jsonField) =>
			{
				BGField bGField = BGField.Create(meta, jsonField.Type, new BGId(jsonField.Id), jsonField.Name, jsonField.Config, jsonField.IsSystem, jsonField.Addon, jsonField.DefaultValue, jsonField.Required);
				bGField.CustomStringFormatterTypeAsString = (string.IsNullOrEmpty(jsonField.StringFormatter) ? null : jsonField.StringFormatter);
				bGField.CustomEditorTypeAsString = jsonField.CustomEditor;
				bGField.Comment = jsonField.Comment;
				bGField.ControllerType = jsonField.ControllerType;
				bGField.UserDefinedReadonly = jsonField.UserDefinedReadonly;
			});
			if (!skipData)
			{
				BGUtil.ForEach(jsonMeta.Entities, (BGJsonRepoModel.Entity jsonEntity) =>
				{
					BGEntity entity = meta.NewEntity(new BGId(jsonEntity.Id));
					BGUtil.ForEach(jsonEntity.Values, (BGJsonRepoModel.FieldValue value) =>
					{
						try
						{
							meta.GetField(value.Name).FromString(entity.Index, value.Value);
						}
						catch (Exception ex)
						{
							Debug.Log(BGUtil.Format("Can not fetch field $ value for entity with id=$. Field Value =$. Error=$", value.Name, entity.Id, value.Value, ex.Message));
							Debug.LogException(ex);
						}
					});
				});
			}
			BGUtil.ForEach(jsonMeta.Keys, (BGJsonRepoModel.Key jsonKey) =>
			{
				if (jsonKey.FieldIds != null && jsonKey.FieldIds.Count != 0)
				{
					List<BGField> list = new List<BGField>();
					foreach (string fieldId in jsonKey.FieldIds)
					{
						BGId bGId = new BGId(fieldId);
						BGField field = meta.GetField(bGId, errorIfNotFound: false);
						if (field == null)
						{
							string name = jsonKey.Name;
							BGId bGId2 = bGId;
							Debug.Log("Can not read key [" + name + "], can not find a field with id=" + bGId2.ToString());
							return;
						}
						list.Add(field);
					}
					BGKey bGKey = BGKey.Create(new BGId(jsonKey.Id), jsonKey.Name, jsonKey.Unique, list.ToArray());
					bGKey.Comment = jsonKey.Comment;
					bGKey.ControllerType = jsonKey.ControllerType;
				}
			});
			BGUtil.ForEach(jsonMeta.Indexes, (BGJsonRepoModel.Index jsonIndex) =>
			{
				BGId bGId = new BGId(jsonIndex.FieldId);
				BGField field = meta.GetField(bGId, errorIfNotFound: false);
				if (field == null)
				{
					string name = jsonIndex.Name;
					BGId bGId2 = bGId;
					Debug.Log("Can not read index [" + name + "], can not find a field with id=" + bGId2.ToString());
				}
				else
				{
					BGIndex bGIndex = BGIndex.Create(new BGId(jsonIndex.Id), jsonIndex.Name, field);
					bGIndex.Comment = jsonIndex.Comment;
					bGIndex.ControllerType = jsonIndex.ControllerType;
				}
			});
		});
		BGUtil.ForEach(model.Views, (BGJsonRepoModel.View jsonView) =>
		{
			BGMetaView view = BGMetaView.Create(repo, new BGId(jsonView.Id), jsonView.Name);
			view.System = jsonView.IsSystem;
			view.Addon = jsonView.Addon;
			view.Comment = jsonView.Comment;
			view.ControllerType = jsonView.ControllerType;
			view.ConfigFromString(jsonView.Config);
			BGRepo bGRepo = new BGRepo();
			Read(jsonView.Repo, bGRepo, skipData: true);
			view.DelegateMeta = (BGMetaRow)bGRepo.GetMeta(view.Id);
			BGUtil.ForEach(jsonView.MetaMappings, (BGJsonRepoModel.MetaMapping jsonMetaMapping) =>
			{
				view.Mappings.Add(new BGId(jsonMetaMapping.MetaId));
			});
		});
	}
}
