using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGJsonWriter
{
	public string Write(BGRepo repo, bool skipData = false, Action<BGJsonRepoModel> beforeWriting = null)
	{
		BGJsonRepoModel bGJsonRepoModel = new BGJsonRepoModel();
		WriteRepo(repo, skipData, bGJsonRepoModel);
		beforeWriting?.Invoke(bGJsonRepoModel);
		return JsonUtility.ToJson(bGJsonRepoModel, prettyPrint: true);
	}

	public static void WriteRepo(BGRepo repo, bool skipData, BGJsonRepoModel model)
	{
		model.ProducedBy = "BGDatabase";
		model.DbVersion = "1.8.9";
		model.DbBuild = "2024.02.09";
		model.Format = BGJsonFormatEnum.Classic;
		repo.Addons.ForEachAddon((BGAddon addon) =>
		{
			model.Addons.Add(new BGJsonRepoModel.Addon
			{
				Config = addon.ConfigToString(),
				Type = addon.GetType().FullName
			});
		});
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGJsonRepoModel.Meta jsonMeta = new BGJsonRepoModel.Meta();
			model.Metas.Add(jsonMeta);
			jsonMeta.Singleton = meta.Singleton;
			jsonMeta.UniqueName = meta.UniqueName;
			jsonMeta.EmptyName = meta.EmptyName;
			jsonMeta.UserDefinedReadonly = meta.UserDefinedReadonly;
			jsonMeta.RowsCount = meta.CountEntities;
			WriteObjMeta(meta, jsonMeta);
			meta.ForEachField((BGField field) =>
			{
				BGJsonRepoModel.Field field2 = new BGJsonRepoModel.Field
				{
					DefaultValue = field.DefaultValue,
					Required = field.Required,
					UserDefinedReadonly = field.UserDefinedReadonly,
					CustomEditor = field.CustomEditorTypeAsString,
					StringFormatter = field.CustomStringFormatterTypeAsString
				};
				jsonMeta.Fields.Add(field2);
				WriteObjMeta(field, field2);
			});
			if (!skipData)
			{
				meta.ForEachEntity((BGEntity entity) =>
				{
					BGJsonRepoModel.Entity jsonEntity = new BGJsonRepoModel.Entity();
					jsonMeta.Entities.Add(jsonEntity);
					jsonEntity.Id = entity.Id.ToString();
					meta.ForEachField((BGField field) =>
					{
						jsonEntity.Values.Add(new BGJsonRepoModel.FieldValue
						{
							Name = field.Name,
							Value = field.ToString(entity.Index)
						});
					});
				});
			}
			meta.ForEachKey((BGKey key) =>
			{
				BGJsonRepoModel.Key keyJson = new BGJsonRepoModel.Key
				{
					Id = key.Id.ToString(),
					Unique = key.IsUnique,
					Name = key.Name,
					Comment = key.Comment,
					ControllerType = key.ControllerType
				};
				jsonMeta.Keys.Add(keyJson);
				key.ForEachField((BGField field) =>
				{
					keyJson.FieldIds.Add(field.Id.ToString());
				});
			});
			meta.ForEachIndex((BGIndex index) =>
			{
				BGJsonRepoModel.Index item = new BGJsonRepoModel.Index
				{
					Id = index.Id.ToString(),
					Name = index.Name,
					FieldId = index.Field.Id.ToString(),
					Comment = index.Comment,
					ControllerType = index.ControllerType
				};
				jsonMeta.Indexes.Add(item);
			});
		});
		repo.ForEachView((BGMetaView view) =>
		{
			BGJsonRepoModel.View view2 = new BGJsonRepoModel.View
			{
				Id = view.Id.ToString(),
				Name = view.Name,
				Addon = view.Addon,
				Comment = view.Comment,
				ControllerType = view.ControllerType,
				Config = view.ConfigToString()
			};
			model.Views.Add(view2);
			BGJsonRepoModel bGJsonRepoModel = new BGJsonRepoModel();
			WriteRepo(view.DelegateMeta.Repo, skipData: true, bGJsonRepoModel);
			view2.Repo = bGJsonRepoModel;
			view.Mappings.Trim();
			BGId[] includedMetas = view.Mappings.IncludedMetas;
			for (int i = 0; i < includedMetas.Length; i++)
			{
				BGId bGId = includedMetas[i];
				view2.MetaMappings.Add(new BGJsonRepoModel.MetaMapping
				{
					MetaId = bGId.ToString()
				});
			}
		});
	}

	private static void WriteObjMeta(BGMetaObject repoObj, BGJsonRepoModel.ObjMeta jsonObj)
	{
		jsonObj.Id = repoObj.Id.ToString();
		jsonObj.Name = repoObj.Name;
		jsonObj.Addon = repoObj.Addon;
		jsonObj.IsSystem = repoObj.System;
		jsonObj.Type = repoObj.GetType().FullName;
		jsonObj.Config = repoObj.ConfigToString();
		jsonObj.Comment = repoObj.Comment;
		jsonObj.ControllerType = repoObj.ControllerType;
	}
}
