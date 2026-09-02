using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "Diagram", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerDiagram")]
public class BGAddonDiagram : BGAddon
{
	[Serializable]
	private class JsonConfig
	{
		public List<JsonConfigMeta> metas = new List<JsonConfigMeta>();
	}

	[Serializable]
	private class JsonConfigMeta
	{
		public string metaId;

		public float X;

		public float Y;
	}

	public class DiagramMetaData
	{
		public readonly BGAddonDiagram addon;

		public readonly BGId metaId;

		private float x;

		private float y;

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				if (x != value)
				{
					x = value;
					if (!addon.suppressEvents)
					{
						addon.FireChange();
					}
				}
			}
		}

		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				if (y != value)
				{
					y = value;
					if (!addon.suppressEvents)
					{
						addon.FireChange();
					}
				}
			}
		}

		public DiagramMetaData(BGAddonDiagram addon, BGId metaId)
		{
			this.addon = addon;
			this.metaId = metaId;
		}

		public DiagramMetaData(BGAddonDiagram addon, BGId metaId, float x, float y)
			: this(addon, metaId)
		{
			X = x;
			Y = y;
		}
	}

	private readonly Dictionary<BGId, DiagramMetaData> metaId2Data = new Dictionary<BGId, DiagramMetaData>();

	private readonly List<DiagramMetaData> dataList = new List<DiagramMetaData>();

	private byte[] configArray;

	private int configVersion;

	private bool suppressEvents;

	public override BGAddon CloneTo(BGRepo repo)
	{
		BGAddonDiagram bGAddonDiagram = new BGAddonDiagram
		{
			configArray = configArray,
			configVersion = configVersion
		};
		foreach (DiagramMetaData data in dataList)
		{
			bGAddonDiagram.Add(data.metaId, data.X, data.Y);
		}
		return bGAddonDiagram;
	}

	public override string ConfigToString()
	{
		InitFromArray();
		List<JsonConfigMeta> list = new List<JsonConfigMeta>();
		foreach (DiagramMetaData data in dataList)
		{
			list.Add(new JsonConfigMeta
			{
				metaId = data.metaId.ToString(),
				X = data.X,
				Y = data.Y
			});
		}
		return JsonUtility.ToJson(new JsonConfig
		{
			metas = list
		});
	}

	public override void ConfigFromString(string config)
	{
		configArray = null;
		Clear();
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		if (jsonConfig?.metas == null)
		{
			return;
		}
		foreach (JsonConfigMeta meta in jsonConfig.metas)
		{
			if (BGId.TryParse(meta.metaId, out var id))
			{
				Add(id, meta.X, meta.Y);
			}
		}
	}

	public override byte[] ConfigToBytes()
	{
		InitFromArray();
		int num = 4 + 24 * dataList.Count;
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(8 + num);
		bGBinaryWriter.AddInt(1);
		BGBinaryWriter writer2 = new BGBinaryWriter(num);
		writer2.AddArray(() =>
		{
			foreach (DiagramMetaData data in dataList)
			{
				writer2.AddId(data.metaId);
				writer2.AddFloat(data.X);
				writer2.AddFloat(data.Y);
			}
		}, dataList.Count);
		bGBinaryWriter.AddByteArray(writer2.ToArray());
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		Clear();
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			configVersion = num;
			configArray = BGUtil.ToArray(bGBinaryReader.ReadByteArray());
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	private void InitFromArray()
	{
		if (configArray == null)
		{
			return;
		}
		try
		{
			suppressEvents = true;
			dataList.Clear();
			metaId2Data.Clear();
			int num = configVersion;
			if (num == 1)
			{
				BGBinaryReader reader = new BGBinaryReader(configArray);
				reader.ReadArray(() =>
				{
					BGId metaId = reader.ReadId();
					float x = reader.ReadFloat();
					float y = reader.ReadFloat();
					Add(metaId, x, y);
				});
				configArray = null;
				return;
			}
			throw new BGException("Unknown version: $", configVersion);
		}
		finally
		{
			suppressEvents = false;
		}
	}

	private DiagramMetaData Add(BGId metaId, float x, float y)
	{
		DiagramMetaData diagramMetaData = new DiagramMetaData(this, metaId, x, y);
		dataList.Add(diagramMetaData);
		metaId2Data[diagramMetaData.metaId] = diagramMetaData;
		return diagramMetaData;
	}

	public DiagramMetaData Get(BGId metaId)
	{
		InitFromArray();
		return BGUtil.Get(metaId2Data, metaId);
	}

	public DiagramMetaData Ensure(BGId metaId)
	{
		DiagramMetaData diagramMetaData = Get(metaId);
		if (diagramMetaData != null)
		{
			return diagramMetaData;
		}
		return Add(metaId, 0f, 0f);
	}

	public void Clear()
	{
		InitFromArray();
		dataList.Clear();
		metaId2Data.Clear();
	}
}
