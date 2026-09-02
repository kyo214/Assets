using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateLoaderUnityWebRequest : BGLiveUpdateLoaderA
{
	public class AsyncLoader : MonoBehaviour
	{
		private LoadContext context;

		private int timeout;

		private Action<Exception> errorAction;

		private Action<DownloadHandler> successAction;

		public void Init(LoadContext context, int timeout, Action<Exception> errorAction, Action<DownloadHandler> successAction)
		{
			this.context = context;
			this.timeout = timeout;
			this.errorAction = errorAction;
			this.successAction = successAction;
		}

		private void Start()
		{
			StartCoroutine(Load());
		}

		private IEnumerator Load()
		{
			Exception ex = null;
			UnityWebRequest webRequest = null;
			try
			{
				switch (context.Method)
				{
				case BGLiveUpdateHttpMethodEnum.Default:
					webRequest = UnityWebRequest.Get(context.Url);
					break;
				case BGLiveUpdateHttpMethodEnum.Get:
				{
					string text = "";
					if (context.httpParameters.Count > 0)
					{
						text += "?";
						foreach (Tuple<string, string> httpParameter in context.httpParameters)
						{
							if (text.Length > 1)
							{
								text += "&";
							}
							text = text + httpParameter.Item1 + "=" + UnityWebRequest.EscapeURL(httpParameter.Item2);
						}
					}
					webRequest = UnityWebRequest.Get(context.Url + text);
					break;
				}
				case BGLiveUpdateHttpMethodEnum.Post:
				{
					WWWForm wWWForm = new WWWForm();
					foreach (Tuple<string, string> httpParameter2 in context.httpParameters)
					{
						wWWForm.AddField(httpParameter2.Item1, httpParameter2.Item2);
					}
					webRequest = UnityWebRequest.Post(context.Url, wWWForm);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
				foreach (Tuple<string, string> httpHeader in context.httpHeaders)
				{
					webRequest.SetRequestHeader(httpHeader.Item1, httpHeader.Item2);
				}
				webRequest.timeout = timeout;
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			if (ex == null)
			{
				yield return webRequest.SendWebRequest();
				try
				{
					if (webRequest.isNetworkError || webRequest.isHttpError)
					{
						throw new Exception("Error while loading: " + ((!string.IsNullOrEmpty(webRequest.error)) ? webRequest.error : "unknown error") + ", response code:" + webRequest.responseCode);
					}
				}
				catch (Exception ex3)
				{
					ex = ex3;
				}
			}
			if (ex != null)
			{
				Debug.LogException(ex);
				errorAction(ex);
			}
			else
			{
				successAction(webRequest.downloadHandler);
			}
		}
	}

	private static GameObject go;

	private readonly Action done;

	private readonly List<LoadContext> textRequests = new List<LoadContext>();

	private readonly List<Action<LoadResultText>> textRequestsCallbacks = new List<Action<LoadResultText>>();

	private readonly List<LoadContext> binaryRequests = new List<LoadContext>();

	private readonly List<Action<LoadResultBinary>> binaryRequestsCallbacks = new List<Action<LoadResultBinary>>();

	private readonly int timeout;

	private int count;

	private static GameObject Go
	{
		get
		{
			if (go != null)
			{
				return go;
			}
			go = new GameObject("BGDatabaseLiveUpdateLoader");
			UnityEngine.Object.DontDestroyOnLoad(go);
			return go;
		}
	}

	public BGLiveUpdateLoaderUnityWebRequest(int timeout, Action done)
	{
		this.timeout = timeout;
		this.done = done;
	}

	public override void Load(LoadContext context, Action<LoadResultText> callback)
	{
		textRequests.Add(context);
		textRequestsCallbacks.Add(callback);
	}

	public override void Load(LoadContext context, Action<LoadResultBinary> callback)
	{
		binaryRequests.Add(context);
		binaryRequestsCallbacks.Add(callback);
	}

	public override void Complete()
	{
		if (textRequests.Count == 0 && binaryRequests.Count == 0)
		{
			return;
		}
		count = textRequests.Count + binaryRequests.Count;
		GameObject gameObject = Go;
		for (int i = 0; i < textRequests.Count; i++)
		{
			LoadContext request = textRequests[i];
			Action<LoadResultText> requestCallback = textRequestsCallbacks[i];
			AsyncLoader asyncLoader = gameObject.AddComponent<AsyncLoader>();
			asyncLoader.Init(request, timeout, (Exception exception) =>
			{
				try
				{
					string localData = "";
					LoadFromLocalFile(request, ref exception, (string file) =>
					{
						localData = File.ReadAllText(file);
					});
					requestCallback((exception != null) ? new LoadResultText(GetMessage(exception), null) : new LoadResultText(null, localData));
				}
				finally
				{
					Done();
				}
			}, (DownloadHandler handler) =>
			{
				try
				{
					requestCallback(new LoadResultText(null, handler.text));
					BGLiveUpdateLoaderA.WriteToLocalFile(request, (string file) =>
					{
						File.WriteAllText(file, handler.text);
					});
				}
				finally
				{
					Done();
				}
			});
		}
		for (int num = 0; num < binaryRequests.Count; num++)
		{
			LoadContext request2 = binaryRequests[num];
			Action<LoadResultBinary> requestCallback2 = binaryRequestsCallbacks[num];
			AsyncLoader asyncLoader2 = gameObject.AddComponent<AsyncLoader>();
			asyncLoader2.Init(request2, timeout, (Exception exception) =>
			{
				try
				{
					byte[] localData = null;
					LoadFromLocalFile(request2, ref exception, (string file) =>
					{
						localData = File.ReadAllBytes(file);
					});
					requestCallback2((exception != null) ? new LoadResultBinary(GetMessage(exception), null) : new LoadResultBinary(null, localData));
				}
				finally
				{
					Done();
				}
			}, (DownloadHandler handler) =>
			{
				try
				{
					requestCallback2(new LoadResultBinary(null, handler.data));
					BGLiveUpdateLoaderA.WriteToLocalFile(request2, (string file) =>
					{
						File.WriteAllBytes(file, handler.data);
					});
				}
				finally
				{
					Done();
				}
			});
		}
	}

	private static string GetMessage(Exception e)
	{
		if (e == null)
		{
			return "unknown error";
		}
		if (e.Message == null)
		{
			return "unknown error: " + e.GetType().FullName;
		}
		return e.Message;
	}

	private void Done()
	{
		count--;
		if (count == 0)
		{
			if (go != null)
			{
				UnityEngine.Object.Destroy(go);
			}
			done();
		}
	}
}
