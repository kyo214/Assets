using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGLiveUpdateUrl
{
	[Serializable]
	public class ParameterWithGraph : ISerializationCallbackReceiver
	{
		public string Name;

		public string Value;

		public string GraphAsString;

		[NonSerialized]
		public BGCalcGraph Graph;

		public string FinalValue
		{
			get
			{
				if (Graph == null)
				{
					return Value;
				}
				BGCalcFlowContext bGCalcFlowContext = BGCalcFlowContext.Get();
				try
				{
					bGCalcFlowContext.Graph = Graph;
					bGCalcFlowContext.GraphType = BGCalcGraphTypeEnum.LiveUpdateHttpParameterValue;
					return Graph.Execute<string>(bGCalcFlowContext);
				}
				finally
				{
					BGCalcFlowContext.Return(bGCalcFlowContext);
				}
			}
		}

		public ParameterWithGraph(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public void OnBeforeSerialize()
		{
			GraphAsString = Graph?.ToJsonString();
		}

		public void OnAfterDeserialize()
		{
			if (string.IsNullOrEmpty(GraphAsString))
			{
				Graph = null;
				return;
			}
			Graph = BGCalcGraph.ExistingGraph();
			Graph.FromJsonString(GraphAsString);
		}
	}

	[Serializable]
	public class HttpParameter : ParameterWithGraph
	{
		public HttpParameter(string name, string value)
			: base(name, value)
		{
		}

		public void CloneTo(BGLiveUpdateUrl url)
		{
			HttpParameter httpParameter = url.AddHttpParameter(Name, Value);
			httpParameter.Graph = Graph;
		}
	}

	[Serializable]
	public class HttpHeader : ParameterWithGraph
	{
		public HttpHeader(string name, string value)
			: base(name, value)
		{
		}

		public HttpHeader CloneTo(BGLiveUpdateUrl url)
		{
			HttpHeader httpHeader = url.AddHttpHeader(Name, Value);
			httpHeader.Graph = Graph;
			return httpHeader;
		}
	}

	[SerializeField]
	private string url;

	[SerializeField]
	private BGLiveUpdateUrlTypeEnum urlType;

	[SerializeField]
	private string metaId;

	[SerializeField]
	private BGLiveUpdateHttpMethodEnum httpMethod;

	[SerializeField]
	private List<HttpParameter> httpParameters;

	[SerializeField]
	private List<HttpHeader> httpHeaders;

	[NonSerialized]
	private BGLiveUpdateUrls urls;

	public BGLiveUpdateUrls Urls
	{
		get
		{
			return urls;
		}
		internal set
		{
			urls = value;
		}
	}

	public string URL
	{
		get
		{
			return url;
		}
		set
		{
			if (!(url == value))
			{
				url = value;
				FireEvent();
			}
		}
	}

	public BGLiveUpdateUrlTypeEnum URLType
	{
		get
		{
			return urlType;
		}
		set
		{
			if (urlType != value)
			{
				urlType = value;
				FireEvent();
			}
		}
	}

	public string MetaId
	{
		get
		{
			return metaId;
		}
		set
		{
			if (!(metaId == value))
			{
				metaId = value;
				FireEvent();
			}
		}
	}

	public BGLiveUpdateHttpMethodEnum HttpMethod
	{
		get
		{
			return httpMethod;
		}
		set
		{
			if (httpMethod != value)
			{
				httpMethod = value;
				FireEvent();
			}
		}
	}

	public List<HttpParameter> HttpParameters => httpParameters;

	public List<HttpHeader> HttpHeaders => httpHeaders;

	public List<Tuple<string, string>> HttpParametersAsTuples
	{
		get
		{
			if (httpParameters == null || httpParameters.Count == 0)
			{
				return null;
			}
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			foreach (HttpParameter httpParameter in httpParameters)
			{
				list.Add(new Tuple<string, string>(httpParameter.Name, httpParameter.FinalValue));
			}
			return list;
		}
	}

	public List<Tuple<string, string>> HttpHeadersAsTuples
	{
		get
		{
			if (httpHeaders == null || httpHeaders.Count == 0)
			{
				return null;
			}
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			foreach (HttpHeader httpHeader in httpHeaders)
			{
				list.Add(new Tuple<string, string>(httpHeader.Name, httpHeader.FinalValue));
			}
			return list;
		}
	}

	public BGLiveUpdateUrl()
	{
	}

	public BGLiveUpdateUrl(BGLiveUpdateUrls urls, string url, BGLiveUpdateUrlTypeEnum urlType, string metaId)
	{
		this.urls = urls;
		this.url = url;
		this.urlType = urlType;
		this.metaId = metaId;
	}

	private void FireEvent()
	{
		urls?.FireEvent();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(url ?? "[No URL]");
		if (httpParameters != null && httpParameters.Count > 0)
		{
			stringBuilder.Append('?');
			foreach (HttpParameter httpParameter in httpParameters)
			{
				stringBuilder.Append(httpParameter.Name + "=" + httpParameter.FinalValue);
			}
		}
		return stringBuilder.ToString();
	}

	public BGLiveUpdateUrl CloneTo(BGLiveUpdateUrls urls)
	{
		BGLiveUpdateUrl result = new BGLiveUpdateUrl(urls, url, urlType, metaId)
		{
			httpMethod = httpMethod
		};
		if (httpParameters != null && httpParameters.Count > 0)
		{
			foreach (HttpParameter httpParameter in httpParameters)
			{
				httpParameter.CloneTo(result);
			}
		}
		if (httpHeaders != null && httpHeaders.Count > 0)
		{
			foreach (HttpHeader httpHeader in httpHeaders)
			{
				httpHeader.CloneTo(result);
			}
		}
		return result;
	}

	public HttpParameter AddHttpParameter(string name, string value)
	{
		httpParameters = httpParameters ?? new List<HttpParameter>();
		HttpParameter httpParameter = new HttpParameter(name, value);
		httpParameters.Add(httpParameter);
		FireEvent();
		return httpParameter;
	}

	public void RemoveHttpParameter(HttpParameter parameter)
	{
		List<HttpParameter> list = httpParameters;
		if (list != null && list.Remove(parameter))
		{
			FireEvent();
		}
	}

	public HttpHeader AddHttpHeader(string name, string value)
	{
		httpHeaders = httpHeaders ?? new List<HttpHeader>();
		HttpHeader httpHeader = new HttpHeader(name, value);
		httpHeaders.Add(httpHeader);
		FireEvent();
		return httpHeader;
	}

	public void RemoveHttpHeader(HttpHeader httpHeader)
	{
		List<HttpHeader> list = httpHeaders;
		if (list != null && list.Remove(httpHeader))
		{
			FireEvent();
		}
	}
}
