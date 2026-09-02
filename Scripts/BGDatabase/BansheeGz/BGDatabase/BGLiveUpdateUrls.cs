using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGLiveUpdateUrls : BGConfigurableBinaryI
{
	public List<BGLiveUpdateUrl> urls;

	private BGAddonLiveUpdate addon;

	public BGAddonLiveUpdate Addon
	{
		get
		{
			return addon;
		}
		set
		{
			addon = value;
			if (urls == null)
			{
				return;
			}
			foreach (BGLiveUpdateUrl url in urls)
			{
				url.Urls = this;
			}
		}
	}

	public BGLiveUpdateUrls()
	{
	}

	public BGLiveUpdateUrls(BGAddonLiveUpdate addon)
	{
		this.addon = addon;
	}

	public byte[] ConfigToBytes()
	{
		BGBinaryWriter writer = new BGBinaryWriter(1024);
		writer.AddInt(2);
		writer.AddArray(() =>
		{
			foreach (BGLiveUpdateUrl url in urls)
			{
				writer.AddString(url.URL);
				writer.AddInt((int)url.URLType);
				writer.AddString(url.MetaId);
				writer.AddByte((byte)url.HttpMethod);
				WriteParameters(writer, url.HttpParameters);
				WriteParameters(writer, url.HttpHeaders);
			}
		}, urls?.Count ?? 0);
		return writer.ToArray();
	}

	public void ConfigFromBytes(ArraySegment<byte> config)
	{
		if (urls != null)
		{
			urls.Clear();
		}
		else
		{
			urls = new List<BGLiveUpdateUrl>();
		}
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		switch (num)
		{
		case 1:
			reader.ReadArray(() =>
			{
				urls.Add(new BGLiveUpdateUrl(this, reader.ReadString(), (BGLiveUpdateUrlTypeEnum)reader.ReadInt(), reader.ReadString()));
			});
			break;
		case 2:
			reader.ReadArray(() =>
			{
				BGLiveUpdateUrl liveUpdateUrl = new BGLiveUpdateUrl(this, reader.ReadString(), (BGLiveUpdateUrlTypeEnum)reader.ReadInt(), reader.ReadString());
				urls.Add(liveUpdateUrl);
				liveUpdateUrl.HttpMethod = (BGLiveUpdateHttpMethodEnum)reader.ReadByte();
				ReadParameters(reader, (string key, string value) => liveUpdateUrl.AddHttpParameter(key, value));
				ReadParameters(reader, (string key, string value) => liveUpdateUrl.AddHttpHeader(key, value));
			});
			break;
		default:
			throw new ArgumentException("wrong version=" + num);
		}
	}

	private static void WriteParameters<T>(BGBinaryWriter writer, List<T> parameters) where T : BGLiveUpdateUrl.ParameterWithGraph
	{
		writer.AddArray(() =>
		{
			foreach (T parameter in parameters)
			{
				writer.AddString(parameter.Name);
				writer.AddString(parameter.Value);
				writer.AddBool(parameter.Graph != null);
				if (parameter.Graph != null)
				{
					writer.AddByteArray(parameter.Graph.ToBytes());
				}
			}
		}, parameters?.Count ?? 0);
	}

	private static void ReadParameters(BGBinaryReader reader, Func<string, string, BGLiveUpdateUrl.ParameterWithGraph> factory)
	{
		reader.ReadArray(() =>
		{
			BGLiveUpdateUrl.ParameterWithGraph parameterWithGraph = factory(reader.ReadString(), reader.ReadString());
			if (reader.ReadBool())
			{
				parameterWithGraph.Graph = BGCalcGraph.ExistingGraph();
				parameterWithGraph.Graph.FromBytes(reader.ReadByteArray());
			}
		});
	}

	public void DeleteUrl(int index)
	{
		if (urls == null || index >= urls.Count || index < 0)
		{
			throw new Exception("Can not delete at specified index " + index + ", number of URLs is  " + (urls?.Count ?? 0));
		}
		urls.RemoveAt(index);
		FireEvent();
	}

	public BGLiveUpdateUrl AddUrl()
	{
		urls = urls ?? new List<BGLiveUpdateUrl>();
		BGLiveUpdateUrl bGLiveUpdateUrl = new BGLiveUpdateUrl();
		urls.Add(bGLiveUpdateUrl);
		return bGLiveUpdateUrl;
	}

	internal void FireEvent()
	{
		addon?.FireChange();
	}

	public BGLiveUpdateUrls CloneTo(BGAddonLiveUpdate addon)
	{
		BGLiveUpdateUrls bGLiveUpdateUrls = new BGLiveUpdateUrls(addon);
		if (urls == null || urls.Count <= 0)
		{
			return bGLiveUpdateUrls;
		}
		bGLiveUpdateUrls.urls = new List<BGLiveUpdateUrl>(urls.Count);
		for (int i = 0; i < urls.Count; i++)
		{
			BGLiveUpdateUrl bGLiveUpdateUrl = urls[i];
			bGLiveUpdateUrls.urls.Add(bGLiveUpdateUrl.CloneTo(bGLiveUpdateUrls));
		}
		return bGLiveUpdateUrls;
	}
}
