using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGDBAutoMapRegistry
{
	private struct AutoMappedConfigKey(BGId metaId, Type targetType, bool includeBaseTypes)
	{
		private readonly BGId metaId = metaId;

		private readonly Type targetType = targetType;

		private readonly bool includeBaseTypes = includeBaseTypes;

		public bool Equals(AutoMappedConfigKey other)
		{
			if (metaId.Equals(other.metaId) && object.Equals(targetType, other.targetType))
			{
				return includeBaseTypes == other.includeBaseTypes;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is AutoMappedConfigKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = metaId.GetHashCode();
			hashCode = (hashCode * 397) ^ ((targetType != null) ? targetType.GetHashCode() : 0);
			int num = hashCode * 397;
			bool flag = includeBaseTypes;
			return num ^ flag.GetHashCode();
		}
	}

	public class AutoMappedConfig
	{
		private readonly BGMetaEntity meta;

		private readonly Type targetType;

		private Dictionary<BGId, FieldInfo> fieldId2field;

		private Dictionary<BGId, PropertyInfo> propertyId2field;

		public int Count => (fieldId2field?.Count ?? 0) + (propertyId2field?.Count ?? 0);

		public AutoMappedConfig(BGMetaEntity meta, Type targetType, bool includeBaseTypes)
		{
			AutoMappedConfig autoMappedConfig = this;
			this.meta = meta;
			this.targetType = targetType;
			meta.ForEachField((BGField field) =>
			{
				string text = field.Name;
				if ("name".Equals(text))
				{
					text = "entityName";
				}
				FieldInfo field2 = BGPrivate.GetField(targetType, text, isStatic: false, includeBaseTypes);
				if (field2 != null)
				{
					autoMappedConfig.fieldId2field = autoMappedConfig.fieldId2field ?? new Dictionary<BGId, FieldInfo>();
					autoMappedConfig.fieldId2field.Add(field.Id, field2);
				}
				else
				{
					PropertyInfo property = BGPrivate.GetProperty(targetType, text, isStatic: false, includeBaseTypes);
					if (property != null)
					{
						autoMappedConfig.propertyId2field = autoMappedConfig.propertyId2field ?? new Dictionary<BGId, PropertyInfo>();
						autoMappedConfig.propertyId2field.Add(field.Id, property);
					}
				}
			});
		}

		public void Bind(BGEntity entity, UnityEngine.Object target, bool detailedLog, Action<string> logger)
		{
			string logMessage = null;
			if (detailedLog)
			{
				logMessage = "Row [" + entity.FullName + "] to [" + target.name + "] object: ";
			}
			BindInternal(entity, logger, (BGField field, FieldInfo info, int index) =>
			{
				object value = field.GetValue(entity.Index);
				info.SetValue(target, value);
				if (detailedLog)
				{
					logMessage += $"[{field.Name}={value}], ";
				}
			}, (BGField field, PropertyInfo info, int index) =>
			{
				object value = field.GetValue(entity.Index);
				info.SetValue(target, value, null);
				if (detailedLog)
				{
					logMessage += $"[{field.Name}={value}], ";
				}
			});
			if (detailedLog)
			{
				logger?.Invoke(logMessage);
			}
		}

		public void ReverseBind(BGEntity entity, UnityEngine.Object target, Action<string> logger)
		{
			BindInternal(entity, logger, (BGField field, FieldInfo info, int index) =>
			{
				field.SetValue(entity.Index, info.GetValue(target));
			}, (BGField field, PropertyInfo info, int index) =>
			{
				field.SetValue(entity.Index, info.GetValue(target, null));
			});
		}

		private void BindInternal(BGEntity entity, Action<string> logger, Action<BGField, FieldInfo, int> action, Action<BGField, PropertyInfo, int> action2)
		{
			if (fieldId2field != null)
			{
				foreach (KeyValuePair<BGId, FieldInfo> item in fieldId2field)
				{
					BGField field = entity.Meta.GetField(item.Key, errorIfNotFound: false);
					if (field != null)
					{
						try
						{
							action(field, item.Value, entity.Index);
						}
						catch (Exception ex)
						{
							logger?.Invoke("DataBinder error: details are [From Meta=" + entity.Meta.Name + ", field=" + field.Name + ", entity=" + entity.Name + "] [To Class=" + targetType.Name + "]  Original error is: " + ex.Message);
						}
					}
				}
			}
			if (propertyId2field == null)
			{
				return;
			}
			foreach (KeyValuePair<BGId, PropertyInfo> item2 in propertyId2field)
			{
				BGField field2 = entity.Meta.GetField(item2.Key, errorIfNotFound: false);
				if (field2 != null)
				{
					try
					{
						action2(field2, item2.Value, entity.Index);
					}
					catch (Exception ex2)
					{
						logger?.Invoke("DataBinder error: details are [From Meta=" + entity.Meta.Name + ", field=" + field2.Name + ", entity=" + entity.Name + "] [To Class=" + targetType.Name + "]  Original error is: " + ex2.Message);
					}
				}
			}
		}

		public void ForEachField(Action<BGId, FieldInfo> action)
		{
			if (fieldId2field == null)
			{
				return;
			}
			foreach (KeyValuePair<BGId, FieldInfo> item in fieldId2field)
			{
				action(item.Key, item.Value);
			}
		}

		public void ForEachProperty(Action<BGId, PropertyInfo> action)
		{
			if (propertyId2field == null)
			{
				return;
			}
			foreach (KeyValuePair<BGId, PropertyInfo> item in propertyId2field)
			{
				action(item.Key, item.Value);
			}
		}
	}

	public const string EntityName = "entityName";

	private readonly Dictionary<AutoMappedConfigKey, AutoMappedConfig> configs = new Dictionary<AutoMappedConfigKey, AutoMappedConfig>();

	private static BGDBAutoMapRegistry instance;

	public static BGDBAutoMapRegistry Instance => instance ?? (instance = new BGDBAutoMapRegistry());

	private BGDBAutoMapRegistry()
	{
	}

	public void Invalidate()
	{
		configs.Clear();
	}

	public AutoMappedConfig GetAutoMappedConfig(BGMetaEntity meta, Type targetComponentType, bool includeBaseTypes)
	{
		AutoMappedConfigKey key = new AutoMappedConfigKey(meta.Id, targetComponentType, includeBaseTypes);
		if (configs.TryGetValue(key, out var value))
		{
			return value;
		}
		value = new AutoMappedConfig(meta, targetComponentType, includeBaseTypes);
		configs.Add(key, value);
		return value;
	}
}
