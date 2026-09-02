using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateLog
{
	public enum LogLevelEnum
	{
		Summary = 0,
		SummaryByMeta = 10,
		Detailed = 20,
		Full = 100
	}

	public enum StatusEnum
	{
		NotLoaded = 0,
		LoadAttempted = 1,
		LoadNotSupported = 2
	}

	public class MetaInfo
	{
		private readonly Dictionary<BGId, HashSet<BGId>> invalidEntityId2FieldIds = new Dictionary<BGId, HashSet<BGId>>();

		private readonly LogLevelEnum level;

		public bool IsError => !string.IsNullOrEmpty(Error);

		public string Error { get; set; }

		public int OkCellsCount { get; set; }

		public int InvalidCellsCount { get; set; }

		public MetaInfo(LogLevelEnum level)
		{
			this.level = level;
		}

		public bool IsEntityHasInvalidValues(BGId entityId)
		{
			CheckMinLevel();
			return invalidEntityId2FieldIds.ContainsKey(entityId);
		}

		public HashSet<BGId> GetInvalidFields(BGId entityId)
		{
			CheckMinLevel();
			return BGUtil.Get(invalidEntityId2FieldIds, entityId);
		}

		public HashSet<BGId> GetInvalidEntities()
		{
			CheckMinLevel();
			HashSet<BGId> hashSet = new HashSet<BGId>();
			foreach (KeyValuePair<BGId, HashSet<BGId>> invalidEntityId2FieldId in invalidEntityId2FieldIds)
			{
				hashSet.Add(invalidEntityId2FieldId.Key);
			}
			return hashSet;
		}

		public bool IsFieldInvalid(BGId entityId, BGId fieldId)
		{
			CheckMinLevel();
			if (!invalidEntityId2FieldIds.TryGetValue(entityId, out var value))
			{
				return false;
			}
			return value?.Contains(fieldId) ?? false;
		}

		private void CheckMinLevel()
		{
			if (level < LogLevelEnum.Detailed)
			{
				throw new Exception("You can not use this method with log level=" + level.ToString() + " . Assign at least 'Detailed' level in LiveUpdate addon settings page!");
			}
		}

		public void AddCellError(BGId entityId, BGId fieldId)
		{
			if (!invalidEntityId2FieldIds.TryGetValue(entityId, out var value))
			{
				value = new HashSet<BGId>();
				invalidEntityId2FieldIds.Add(entityId, value);
			}
			value.Add(fieldId);
		}
	}

	private readonly StringBuilder detailsBuilder = new StringBuilder();

	private readonly LogLevelEnum level;

	private readonly Dictionary<BGId, MetaInfo> id2MetaInfo = new Dictionary<BGId, MetaInfo>();

	private readonly List<string> errors = new List<string>();

	public string Exception;

	public int OkMetaCount { get; set; }

	public int InvalidMetaCount { get; set; }

	public int OkCellsCount { get; set; }

	public int InvalidCellsCount { get; set; }

	public BGRepo Repo { get; set; }

	public StatusEnum Status { get; set; }

	public LogLevelEnum Level => level;

	public BGLiveUpdateLog(LogLevelEnum level)
	{
		this.level = level;
	}

	public bool IsMetaLoadedOk(BGId metaId)
	{
		if (Status != StatusEnum.LoadAttempted)
		{
			return false;
		}
		if (Exception != null)
		{
			return false;
		}
		MetaInfo metaDetails = GetMetaDetails(metaId);
		if (metaDetails == null)
		{
			return true;
		}
		return !metaDetails.IsError;
	}

	public void SetError(BGId metaId, Exception exception)
	{
		string text = exception.Message ?? exception.GetType().FullName;
		errors.Add(text);
		MetaInfo metaInfo = EnsureMetaInfo(metaId);
		metaInfo.Error = text;
		if (level == LogLevelEnum.Full)
		{
			AddDetail("ERROR STACKTRACE:" + exception.StackTrace);
		}
	}

	private MetaInfo EnsureMetaInfo(BGId metaId)
	{
		if (id2MetaInfo.TryGetValue(metaId, out var value))
		{
			return value;
		}
		value = new MetaInfo(level);
		id2MetaInfo.Add(metaId, value);
		return value;
	}

	public MetaInfo GetMetaDetails(BGId metaId)
	{
		return BGUtil.Get(id2MetaInfo, metaId);
	}

	public void AddDetail(string message, params object[] parameters)
	{
		if (level >= LogLevelEnum.Detailed)
		{
			detailsBuilder.AppendLine(BGUtil.Format(message, parameters));
		}
	}

	public void AddWarning(string message, params object[] parameters)
	{
		AddDetail("WARNING:" + message, parameters);
	}

	public void AddFullDetail(string message, params object[] parameters)
	{
		if (level >= LogLevelEnum.Full)
		{
			AddDetail(message, parameters);
		}
	}

	public void AddCellFailed(BGId metaId, BGId entityId, BGId fieldId, string message, params object[] parameters)
	{
		InvalidCellsCount++;
		if (level >= LogLevelEnum.SummaryByMeta)
		{
			MetaInfo metaInfo = EnsureMetaInfo(metaId);
			metaInfo.InvalidCellsCount++;
			metaInfo.AddCellError(entityId, fieldId);
			if (level >= LogLevelEnum.Detailed)
			{
				AddWarning(message, parameters);
			}
		}
	}

	public void AddCellSuccess(BGId metaId, string message, params object[] parameters)
	{
		OkCellsCount++;
		if (level >= LogLevelEnum.SummaryByMeta)
		{
			EnsureMetaInfo(metaId).OkCellsCount++;
			if (level >= LogLevelEnum.Full)
			{
				AddDetail(message, parameters);
			}
		}
	}

	public void PrintToConsole()
	{
		Debug.Log(GetLog());
	}

	public string GetLog()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("BGDatabase.LiveUpdate plugin log: ");
		stringBuilder.Append(" Status: ").Append(Status);
		stringBuilder.Append(", Ok meta count: ").Append(OkMetaCount);
		stringBuilder.Append(", Invalid meta count: ").Append(InvalidMetaCount);
		stringBuilder.Append(", Ok cells count: ").Append(OkCellsCount);
		stringBuilder.Append(", Invalid cells count: ").Append(InvalidCellsCount);
		stringBuilder.AppendLine();
		if (!string.IsNullOrEmpty(Exception))
		{
			stringBuilder.Append("Global ERROR: " + Exception);
		}
		if (level >= LogLevelEnum.SummaryByMeta)
		{
			int num = 0;
			foreach (KeyValuePair<BGId, MetaInfo> item in id2MetaInfo)
			{
				BGMetaEntity meta = Repo.GetMeta(item.Key);
				if (meta == null)
				{
					stringBuilder.AppendLine(BGUtil.Format("Meta: id=$. Error: can not find meta in repo", item.Key));
					continue;
				}
				MetaInfo value = item.Value;
				stringBuilder.Append(BGUtil.Format("Meta Summary # $. Name $. Status: $ ", num++, meta.Name, (value.Error != null) ? ("ERROR: " + value.Error) : "Loaded"));
				stringBuilder.Append(", Ok cells count: ").Append(value.OkCellsCount);
				stringBuilder.Append(", Invalid cells count: ").Append(value.InvalidCellsCount);
				stringBuilder.AppendLine();
			}
		}
		if (level >= LogLevelEnum.Detailed)
		{
			stringBuilder.AppendLine("Detailed log:");
			stringBuilder.AppendLine(detailsBuilder.ToString());
		}
		return stringBuilder.ToString();
	}

	public void Clear()
	{
		Status = StatusEnum.NotLoaded;
		Exception = null;
		detailsBuilder.Length = 0;
		id2MetaInfo.Clear();
		errors.Clear();
		OkMetaCount = 0;
		InvalidMetaCount = 0;
		OkCellsCount = 0;
		InvalidCellsCount = 0;
	}
}
