using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor;

[AddComponentMenu("Reactor/Progressor Group")]
public class ProgressorGroup : MonoBehaviour
{
	private const float TOLERANCE = 0.001f;

	[SerializeField]
	protected float Progress;

	[SerializeField]
	private List<Progressor> Progressors = new List<Progressor>();

	[SerializeField]
	private List<ProgressTarget> ProgressTargets;

	[SerializeField]
	protected FloatEvent OnProgressChanged = new FloatEvent();

	[SerializeField]
	protected FloatEvent OnProgressIncremented = new FloatEvent();

	[SerializeField]
	protected FloatEvent OnProgressDecremented = new FloatEvent();

	[SerializeField]
	protected UnityEvent OnProgressReachedOne = new UnityEvent();

	[SerializeField]
	protected UnityEvent OnProgressReachedZero = new UnityEvent();

	public List<Progressor> progressors
	{
		get
		{
			Initialize();
			return Progressors;
		}
	}

	public List<ProgressTarget> progressTargets
	{
		get
		{
			Initialize();
			return ProgressTargets;
		}
	}

	public float progress
	{
		get
		{
			return Progress;
		}
		private set
		{
			Progress = Mathf.Clamp01(value);
			OnProgressChanged?.Invoke(Progress);
		}
	}

	public FloatEvent onProgressChanged => OnProgressChanged;

	public FloatEvent onProgressIncremented => OnProgressIncremented;

	public FloatEvent onProgressDecremented => OnProgressDecremented;

	public UnityEvent onProgressReachedOne => OnProgressReachedOne;

	public UnityEvent onProgressReachedZero => OnProgressReachedZero;

	private Coroutine updateCoroutine { get; set; }

	public bool initialized { get; set; }

	public void Initialize()
	{
		if (!initialized)
		{
			if (ProgressTargets == null)
			{
				ProgressTargets = new List<ProgressTarget>();
			}
			if (Progressors == null)
			{
				Progressors = new List<Progressor>();
			}
			initialized = true;
		}
	}

	public void UpdateProgress()
	{
		if (Progressors.Count == 0)
		{
			return;
		}
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < Progressors.Count; i++)
		{
			if (!(Progressors[i] == null))
			{
				num += Progressors[i].progress;
				num2++;
			}
		}
		float num3 = num / (float)num2;
		if (num3 < 0.001f)
		{
			num3 = 0f;
		}
		if (num3 > 0.999f)
		{
			num3 = 1f;
		}
		if (!num3.Approximately(progress, 0.001f))
		{
			float num4 = progress;
			progress = num3;
			if (num4 < progress)
			{
				OnProgressIncremented?.Invoke(progress - num4);
			}
			else if (num4 > progress)
			{
				OnProgressDecremented?.Invoke(num4 - progress);
			}
			if (progress.Approximately(1f, 0.001f))
			{
				OnProgressReachedOne?.Invoke();
			}
			else if (progress.Approximately(0f, 0.001f))
			{
				OnProgressReachedZero?.Invoke();
			}
			ProgressTargets.RemoveNulls();
			ProgressTargets.ForEach((ProgressTarget t) =>
			{
				t.UpdateTarget(this);
			});
		}
	}

	private void Awake()
	{
		initialized = false;
		progress = -1f;
		Initialize();
	}

	private void OnEnable()
	{
		if (Progressors == null)
		{
			Progressors = new List<Progressor>();
		}
		StartUpdate();
	}

	private void OnDisable()
	{
		StopUpdate();
	}

	private void StartUpdate()
	{
		if (updateCoroutine == null)
		{
			updateCoroutine = StartCoroutine(UpdateCoroutine());
		}
	}

	private void StopUpdate()
	{
		if (updateCoroutine != null)
		{
			StopCoroutine(updateCoroutine);
			updateCoroutine = null;
		}
	}

	private IEnumerator UpdateCoroutine()
	{
		float runtimeTickInterval = ReactorSettings.GetRuntimeTickInterval();
		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(runtimeTickInterval);
		while (true)
		{
			yield return wait;
			UpdateProgress();
		}
	}

	private void ValidateTargets()
	{
		ProgressTargets = progressTargets.Where((ProgressTarget t) => t != null).Distinct().ToList();
	}
}
