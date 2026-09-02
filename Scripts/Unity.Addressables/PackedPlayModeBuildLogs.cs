using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PackedPlayModeBuildLogs
{
	[Serializable]
	public struct RuntimeBuildLog(LogType type, string message)
	{
		public LogType Type = type;

		public string Message = message;
	}

	[SerializeField]
	private List<RuntimeBuildLog> m_RuntimeBuildLogs = new List<RuntimeBuildLog>();

	public List<RuntimeBuildLog> RuntimeBuildLogs
	{
		get
		{
			return m_RuntimeBuildLogs;
		}
		set
		{
			m_RuntimeBuildLogs = value;
		}
	}
}
