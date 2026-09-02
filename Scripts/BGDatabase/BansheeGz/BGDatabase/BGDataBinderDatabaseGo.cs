using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderDatabaseGo")]
public class BGDataBinderDatabaseGo : BGDataBinderGoA
{
	public class BinderData
	{
		public readonly BGId EntityId;

		public readonly UnityEngine.Object Target;

		public BinderData(BGId entityId, UnityEngine.Object target)
		{
			EntityId = entityId;
			Target = target;
		}
	}

	public abstract class BinderConfigurationA
	{
		protected readonly Dictionary<FieldInfo, Type> field2Type = new Dictionary<FieldInfo, Type>();

		public void ForEach(Action<FieldInfo, Type> action)
		{
			foreach (KeyValuePair<FieldInfo, Type> item in field2Type)
			{
				action(item.Key, item.Value);
			}
		}

		public abstract UnityEngine.Object[] Find(Type type);

		protected static void Log(string message)
		{
			Debug.Log("BGDataBinderDatabaseGo [debugMode=on]: " + message);
		}
	}

	public abstract class BinderConfigurationA<T> : BinderConfigurationA where T : UnityEngine.Object
	{
		protected BinderConfigurationA()
		{
			List<Type> allSubTypes = BGUtil.GetAllSubTypes(typeof(T));
			Type typeFromHandle = typeof(string);
			for (int i = 0; i < allSubTypes.Count; i++)
			{
				Type type = allSubTypes[i];
				FieldInfo field = BGPrivate.GetField(type, "BGDatabaseEntityId", isStatic: false);
				if (!(field == null) && !(field.FieldType != typeFromHandle))
				{
					field2Type.Add(field, type);
				}
			}
		}

		public void Bind(List<BinderData> binders, bool debugMode)
		{
			BindInternal(binders, debugMode, (BGDBAutoMapRegistry.AutoMappedConfig config, BGEntity entity, T o, Action<string> logger) =>
			{
				config.Bind(entity, o, detailedLog: false, logger);
			});
		}

		public void ReverseBind(List<BinderData> binders, bool debugMode)
		{
			BindInternal(binders, debugMode, (BGDBAutoMapRegistry.AutoMappedConfig config, BGEntity entity, T o, Action<string> logger) =>
			{
				config.ReverseBind(entity, o, logger);
			});
		}

		private void BindInternal(List<BinderData> binders, bool debugMode, Action<BGDBAutoMapRegistry.AutoMappedConfig, BGEntity, T, Action<string>> action)
		{
			if (!BGRepo.Ok)
			{
				Debug.LogError("BGDataBinderDatabaseGo: Can not bind, cause database is not loaded. Error: " + BGRepo.DefaultRepoErrorOnLoad);
				return;
			}
			Action<string> action2 = (debugMode ? new Action<string>(BinderConfigurationA.Log) : null);
			int num = 0;
			foreach (KeyValuePair<FieldInfo, Type> item in field2Type)
			{
				FieldInfo key = item.Key;
				Type value = item.Value;
				UnityEngine.Object[] array = Find(value);
				if (array == null)
				{
					continue;
				}
				for (int i = 0; i < array.Length; i++)
				{
					T val = (T)array[i];
					string text = (string)key.GetValue(val);
					BGId entityId = BGId.Parse(text);
					if (entityId.IsEmpty)
					{
						if (debugMode)
						{
							if (string.IsNullOrEmpty(text))
							{
								BinderConfigurationA.Log("Empty BGDatabaseEntityId value at " + ToString(val));
							}
							else
							{
								BinderConfigurationA.Log("Invalid BGDatabaseEntityId value [" + text + "] at " + ToString(val));
							}
						}
						continue;
					}
					BGEntity entity = BGRepo.I.GetEntity(entityId);
					if (entity == null)
					{
						if (debugMode)
						{
							BinderConfigurationA.Log("Can not find entity with id [" + text + "], defined at " + ToString(val));
						}
					}
					else
					{
						binders?.Add(new BinderData(entityId, val));
						BGDBAutoMapRegistry.AutoMappedConfig autoMappedConfig = BGDBAutoMapRegistry.Instance.GetAutoMappedConfig(entity.Meta, value, includeBaseTypes: true);
						action(autoMappedConfig, entity, val, action2);
						num++;
					}
				}
			}
			action2?.Invoke(num + " objects processed.");
		}

		protected abstract string ToString(T obj);
	}

	public class BinderConfigurationMB : BinderConfigurationA<MonoBehaviour>
	{
		public override UnityEngine.Object[] Find(Type type)
		{
			return UnityEngine.Object.FindObjectsOfType(type);
		}

		protected override string ToString(MonoBehaviour obj)
		{
			Transform transform = obj.transform;
			string text = transform.name;
			while (transform.parent != null)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "GameObject [" + text + "]";
		}
	}

	public const string IdFieldName = "BGDatabaseEntityId";

	private static BGDataBinderDatabaseGo last;

	private static BinderConfigurationMB binderConfigMB;

	[SerializeField]
	private bool debugMode;

	[SerializeField]
	private bool liveUpdate;

	private List<BinderData> bindersMB;

	private bool listenersWasAdded;

	public static BinderConfigurationMB BinderConfigMB => binderConfigMB ?? (binderConfigMB = new BinderConfigurationMB());

	public override string Error => null;

	public bool DebugMode => debugMode;

	public bool LiveUpdate => liveUpdate;

	protected bool IsLiveUpdateOn
	{
		get
		{
			if (liveUpdate)
			{
				if (!Application.isPlaying)
				{
					return BGUtil.TestIsRunning;
				}
				return true;
			}
			return false;
		}
	}

	protected override void FirstBind()
	{
		if (last != null)
		{
			Debug.Log("WARNING! BGDataBinderDatabaseGo: you have more than 1 instance of BGDataBinderDatabaseGo in your scene. This is not optimal, cause 1 is enough");
		}
		last = this;
		if (!liveUpdate)
		{
			Bind();
			return;
		}
		bindersMB = new List<BinderData>();
		BinderConfigMB.Bind(bindersMB, debugMode);
		AddListeners();
	}

	public override void Bind()
	{
		if (!bindedOnce)
		{
			bindedOnce = true;
			FirstBind();
		}
		else
		{
			BinderConfigMB.Bind(null, debugMode);
		}
		FireOnBind();
	}

	public override void ReverseBind()
	{
		BinderConfigMB.ReverseBind(null, debugMode);
	}

	protected override void OnDestroy()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		if (IsLiveUpdateOn && !listenersWasAdded)
		{
			listenersWasAdded = true;
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			AddListeners(bindersMB);
		}
	}

	private void AddListeners(List<BinderData> binders)
	{
		foreach (BinderData binder in binders)
		{
			BGRepo.I.GetEntity(binder.EntityId)?.Meta.AddEntityUpdatedListener(binder.EntityId, EntityIsChanged);
		}
	}

	private void RemoveListeners()
	{
		if (listenersWasAdded)
		{
			BGRepo.OnLoad -= OnLoad;
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			RemoveListeners(bindersMB);
		}
	}

	private void RemoveListeners(List<BinderData> binders)
	{
		if (binders == null)
		{
			return;
		}
		foreach (BinderData binder in binders)
		{
			BGRepo.I.GetEntity(binder.EntityId)?.Meta.RemoveEntityUpdatedListener(binder.EntityId, EntityIsChanged);
		}
	}

	private void EntityIsChanged(object sender, BGEventArgsEntityUpdated e)
	{
		EntityIsChanged(bindersMB, e.Entity);
	}

	private void EntityIsChanged(List<BinderData> binders, BGEntity entity)
	{
		if (entity == null)
		{
			return;
		}
		foreach (BinderData binder in binders)
		{
			if (!(binder.EntityId != entity.Id) && !(binder.Target == null))
			{
				BGDBAutoMapRegistry.Instance.GetAutoMappedConfig(entity.Meta, binder.Target.GetType(), includeBaseTypes: true).Bind(entity, binder.Target, detailedLog: false, null);
			}
		}
	}

	private void OnLoad(bool loaded)
	{
		if (loaded)
		{
			Bind();
		}
	}

	private void OnBatch(object sender, BGEventArgsBatch e)
	{
		Bind();
	}

	public static BGId GetId(UnityEngine.Object asset, ref FieldInfo fieldInfo)
	{
		try
		{
			if (fieldInfo == null)
			{
				fieldInfo = BGPrivate.GetField(asset.GetType(), "BGDatabaseEntityId");
			}
			return BGId.Parse((string)fieldInfo.GetValue(asset));
		}
		catch
		{
			return BGId.Empty;
		}
	}
}
