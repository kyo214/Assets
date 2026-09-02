using UnityEngine;
using UnityEngine.VFX;

public class EliteFlameSequence : MonoBehaviour
{
	[SerializeField]
	private int _sortingOrder;

	[SerializeField]
	private VisualEffect _innerFlame;

	[SerializeField]
	private VisualEffect _outerFlame;

	[SerializeField]
	private Renderer _flameBurst;

	[SerializeField]
	private Renderer _smoke;

	private Animator _animator;

	private VisualEffect _flameBurstFx;

	private VisualEffect _smokeFx;

	private void OnValidate()
	{
		if ((bool)_smoke)
		{
			_smoke.sortingOrder = _sortingOrder;
		}
		if ((bool)_flameBurst)
		{
			_flameBurst.sortingOrder = _sortingOrder;
		}
	}

	private void Start()
	{
		if ((bool)_smoke)
		{
			_smokeFx = _smoke.GetComponent<VisualEffect>();
		}
		if ((bool)_flameBurst)
		{
			_flameBurstFx = _flameBurst.GetComponent<VisualEffect>();
		}
		_animator = base.gameObject.GetComponent<Animator>();
		_outerFlame.enabled = false;
		_flameBurst.enabled = false;
		_smoke.enabled = false;
		if (_innerFlame != null)
		{
			_innerFlame.enabled = false;
		}
	}

	public void Spawn()
	{
		_animator.CrossFade("FlameStart", 0f);
	}

	public void FadeOut()
	{
		_animator.CrossFade("FlameEnd", 0f);
	}

	public void StopSpawner()
	{
		_smokeFx.Stop();
		_flameBurstFx.Stop();
	}
}
