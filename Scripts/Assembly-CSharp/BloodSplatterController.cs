using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class BloodSplatterController : MonoBehaviour
{
	[SerializeField]
	private VisualEffect _particleSpawner;

	[SerializeField]
	private List<Sprite> _decalSpriteRef;

	[SerializeField]
	private SpriteRenderer _decalSprite;

	[SerializeField]
	private float _castDistance;

	[SerializeField]
	private float _showDecalsDelay;

	[SerializeField]
	private float _decalLifetime;

	[SerializeField]
	private float _decalFadeTime;

	[SerializeField]
	private LayerMask _floorMask;

	[SerializeField]
	private bool _testLooping;

	private Color _decalOriginColor;

	private Color _transparent;

	private void Start()
	{
		_particleSpawner.gameObject.SetActive(value: false);
		_decalSprite.gameObject.SetActive(value: false);
		_decalOriginColor = _decalSprite.color;
		_transparent = new Color(_decalOriginColor.r, _decalOriginColor.g, _decalOriginColor.b, 0f);
		if (_testLooping)
		{
			InvokeRepeating("SpawnParticle", 5f, 5f);
		}
	}

	public void SpawnParticle()
	{
		_decalSprite.sprite = _decalSpriteRef[Random.Range(0, _decalSpriteRef.Count)];
		_particleSpawner.gameObject.SetActive(value: true);
		_particleSpawner.Play();
		UniTaskUtil.DelayedCall(this, _showDecalsDelay, () =>
		{
			SpawnSplatter();
		}).Forget();
	}

	private void SpawnSplatter()
	{
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.down), out var hitInfo, _castDistance, _floorMask))
		{
			_decalSprite.gameObject.SetActive(value: true);
			_decalSprite.transform.position = new Vector3(_decalSprite.transform.position.x, hitInfo.transform.position.y, _decalSprite.transform.position.z);
			_decalSprite.color = _decalOriginColor;
			UniTaskUtil.DelayedCall(this, _decalLifetime, () =>
			{
				RemoveSplatter();
			}).Forget();
		}
	}

	private void RemoveSplatter()
	{
		_particleSpawner.gameObject.SetActive(value: false);
		_decalSprite.DOColor(_transparent, _decalFadeTime).OnComplete(() =>
		{
			_decalSprite.gameObject.SetActive(value: false);
		});
	}
}
