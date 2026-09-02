using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateLoaderWebClient : BGLiveUpdateLoaderA
{
	public class MyWebClient : WebClient
	{
		public int Timeout { private get; set; }

		protected override WebRequest GetWebRequest(Uri uri)
		{
			WebRequest webRequest = base.GetWebRequest(uri);
			if (webRequest == null)
			{
				return null;
			}
			webRequest.Timeout = Timeout;
			((HttpWebRequest)webRequest).ReadWriteTimeout = Timeout;
			return webRequest;
		}
	}

	private readonly MyWebClient client;

	public BGLiveUpdateLoaderWebClient(int timeOut)
	{
		client = new MyWebClient
		{
			Timeout = timeOut
		};
	}

	public void LoadBinary(LoadContext context, Action<LoadResultBinary> callback)
	{
		Load(context, callback);
	}

	public void LoadText(LoadContext context, Action<LoadResultText> callback)
	{
		Load(context, callback);
	}

	public override void Load(LoadContext context, Action<LoadResultText> callback)
	{
		string result = null;
		Exception ex = null;
		try
		{
			switch (context.Method)
			{
			case BGLiveUpdateHttpMethodEnum.Default:
				result = client.DownloadString(context.Url);
				break;
			case BGLiveUpdateHttpMethodEnum.Get:
			case BGLiveUpdateHttpMethodEnum.Post:
				result = LoadString(context, context.Method);
				break;
			default:
				throw new ArgumentOutOfRangeException("Method");
			}
			BGLiveUpdateLoaderA.WriteToLocalFile(context, (string file) =>
			{
				File.WriteAllText(file, result);
			});
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			LoadFromLocalFile(context, ref e, (string file) =>
			{
				result = File.ReadAllText(file);
			});
			ex = e;
		}
		finally
		{
			callback((ex == null) ? new LoadResultText(null, result) : new LoadResultText(ex.Message ?? ("unknown error " + ex.GetType().FullName), null));
		}
	}

	public override void Load(LoadContext context, Action<LoadResultBinary> callback)
	{
		byte[] result = null;
		Exception ex = null;
		try
		{
			switch (context.Method)
			{
			case BGLiveUpdateHttpMethodEnum.Default:
				result = client.DownloadData(context.Url);
				break;
			case BGLiveUpdateHttpMethodEnum.Get:
			case BGLiveUpdateHttpMethodEnum.Post:
				result = LoadByteArray(context, context.Method);
				break;
			default:
				throw new ArgumentOutOfRangeException("Method");
			}
			BGLiveUpdateLoaderA.WriteToLocalFile(context, (string file) =>
			{
				File.WriteAllBytes(file, result);
			});
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			LoadFromLocalFile(context, ref e, (string file) =>
			{
				result = File.ReadAllBytes(file);
			});
			ex = e;
		}
		finally
		{
			callback((ex == null) ? new LoadResultBinary(null, result) : new LoadResultBinary(ex.Message ?? ("unknown error " + ex.GetType().FullName), null));
		}
	}

	private string LoadString(LoadContext context, BGLiveUpdateHttpMethodEnum method)
	{
		byte[] bytes = LoadByteArray(context, method);
		return Encoding.UTF8.GetString(bytes);
	}

	private byte[] LoadByteArray(LoadContext context, BGLiveUpdateHttpMethodEnum method)
	{
		if (context.httpHeaders != null && context.httpHeaders.Count > 0)
		{
			foreach (Tuple<string, string> httpHeader in context.httpHeaders)
			{
				client.Headers[httpHeader.Item1] = httpHeader.Item2;
			}
		}
		byte[] result;
		switch (method)
		{
		case BGLiveUpdateHttpMethodEnum.Get:
			client.QueryString.Clear();
			if (context.httpParameters != null && context.httpParameters.Count > 0)
			{
				foreach (Tuple<string, string> httpParameter in context.httpParameters)
				{
					client.QueryString.Add(httpParameter.Item1, httpParameter.Item2);
				}
			}
			result = client.DownloadData(context.Url);
			client.QueryString.Clear();
			break;
		case BGLiveUpdateHttpMethodEnum.Post:
		{
			NameValueCollection nameValueCollection = null;
			if (context.httpParameters != null && context.httpParameters.Count > 0)
			{
				nameValueCollection = new NameValueCollection();
				foreach (Tuple<string, string> httpParameter2 in context.httpParameters)
				{
					nameValueCollection[httpParameter2.Item1] = httpParameter2.Item2;
				}
			}
			result = client.UploadValues(context.Url, "POST", nameValueCollection);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("method", method, $"Unsupported HTTP method {method}");
		}
		return result;
	}

	public override void Complete()
	{
		client?.Dispose();
	}
}
