using UnityEngine;

namespace Chronos;

[AddComponentMenu("Time/Timeline Child")]
[DisallowMultipleComponent]
[HelpURL("http://ludiq.io/chronos/documentation#Timeline")]
public class TimelineChild : TimelineEffector
{
	[SerializeField]
	private bool _initalizedParentManual;

	public Timeline parent { get; private set; }

	protected override Timeline timeline => parent;

	protected override void Awake()
	{
		if (!_initalizedParentManual)
		{
			Init();
		}
	}

	public void CacheParent()
	{
		Timeline timeline = parent;
		parent = GetComponentInParent<Timeline>();
		if (parent == null)
		{
			throw new ChronosException("Missing parent timeline for timeline child.");
		}
		if (timeline != null)
		{
			timeline.children.Remove(this);
		}
		parent.children.Add(this);
	}

	public void Init()
	{
		CacheParent();
		base.Awake();
	}
}
