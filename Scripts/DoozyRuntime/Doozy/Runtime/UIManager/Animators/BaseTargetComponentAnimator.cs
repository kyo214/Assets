using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseTargetComponentAnimator<T> : MonoBehaviour where T : MonoBehaviour
{
	protected RectTransform m_RectTransform;

	[SerializeField]
	private T Controller;

	public RectTransform rectTransform
	{
		get
		{
			if (!m_RectTransform)
			{
				return m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public T controller => Controller;

	public bool hasController => controller != null;

	public bool isConnected { get; protected set; }

	protected Coroutine connectLater { get; set; }

	public bool animatorInitialized { get; set; }

	public virtual void SetController(T newTarget)
	{
		if (hasController && isConnected)
		{
			Disconnect();
		}
		Controller = newTarget;
		if (hasController && base.isActiveAndEnabled)
		{
			Connect();
		}
	}

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			animatorInitialized = false;
			m_RectTransform = GetComponent<RectTransform>();
			UpdateSettings();
			ConnectToController();
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying && !isConnected)
		{
			if (connectLater != null)
			{
				StopCoroutine(connectLater);
				connectLater = null;
			}
			connectLater = StartCoroutine(ConnectLater());
		}
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnDestroy()
	{
		if (Application.isPlaying)
		{
			Disconnect();
		}
	}

	public virtual void InitializeAnimator()
	{
		UpdateSettings();
		animatorInitialized = true;
	}

	protected virtual void Connect()
	{
		if (hasController && !isConnected)
		{
			ConnectToController();
			isConnected = true;
		}
	}

	protected virtual void Disconnect()
	{
		if (hasController && isConnected)
		{
			DisconnectFromController();
			StopAllReactions();
			isConnected = false;
		}
	}

	protected IEnumerator ConnectLater()
	{
		yield return new WaitForEndOfFrame();
		if (!isConnected)
		{
			InitializeAnimator();
			Connect();
		}
	}

	protected abstract void ConnectToController();

	protected abstract void DisconnectFromController();

	public abstract void UpdateSettings();

	public abstract void StopAllReactions();

	public abstract void ResetToStartValues(bool forced = false);

	public abstract List<Heartbeat> SetHeartbeat<Theartbeat>() where Theartbeat : Heartbeat, new();
}
