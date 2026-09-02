using UnityEngine;

namespace DestroyIt;

public class TagAfter : MonoBehaviour
{
	public float seconds = 1f;

	public Tag tagWith;

	private float _timeLeft;

	private bool _isInitialized;

	private void Start()
	{
		_timeLeft = seconds;
		_isInitialized = true;
	}

	private void OnEnable()
	{
		_timeLeft = seconds;
	}

	private void Update()
	{
		if (!_isInitialized)
		{
			return;
		}
		_timeLeft -= Time.deltaTime;
		if (_timeLeft <= 0f)
		{
			Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.AddTag(tagWith);
			}
			Object.Destroy(this);
		}
	}
}
