using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterTrail : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] _spriteInputs;

	[SerializeField]
	private int _trailLength;

	[SerializeField]
	private float _lifeTime;

	[SerializeField]
	private float _delayTime;

	[SerializeField]
	private Transform _scaleRef;

	[SerializeField]
	private float startOpacity = 0.6f;

	[SerializeField]
	private bool isDisable;

	private bool _isTrailing;

	private SpriteRenderer[][] _trailRenderer;

	private GameObject _trailGroup;

	public void InitTrails()
	{
		_trailGroup = new GameObject("VFXTrailGroup");
		_trailGroup.AddComponent<SortingGroup>();
		_trailRenderer = new SpriteRenderer[_trailLength][];
		for (int i = 0; i < _trailLength; i++)
		{
			_trailRenderer[i] = new SpriteRenderer[_spriteInputs.Length];
			for (int j = 0; j < _spriteInputs.Length; j++)
			{
				GameObject gameObject = Object.Instantiate(_spriteInputs[j].gameObject, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = _trailGroup.transform;
				gameObject.transform.position = _spriteInputs[j].transform.position;
				gameObject.transform.rotation = _spriteInputs[j].transform.rotation;
				gameObject.transform.localScale = _scaleRef.transform.localScale;
				_trailRenderer[i][j] = gameObject.GetComponent<SpriteRenderer>();
				gameObject.SetActive(value: false);
				_trailRenderer[i][j].sprite = _spriteInputs[j].sprite;
			}
		}
		_trailGroup.GetComponent<SortingGroup>().sortingOrder = -1;
	}

	public void StartTrail()
	{
		if (isDisable || _isTrailing)
		{
			return;
		}
		_isTrailing = true;
		for (int i = 0; i < _trailLength; i++)
		{
			int cpi = i;
			UniTaskUtil.DelayedCall(this, (float)i * _delayTime, () =>
			{
				SpawnSingleTrail(cpi);
			}).Forget();
		}
	}

	public void StopTrail()
	{
		_isTrailing = false;
	}

	private void SpawnSingleTrail(int idx)
	{
		if (!_isTrailing)
		{
			return;
		}
		int num = 0;
		SpriteRenderer[] array = _trailRenderer[idx];
		foreach (SpriteRenderer renderer in array)
		{
			if (renderer != null)
			{
				renderer.gameObject.SetActive(value: true);
				renderer.transform.position = _spriteInputs[num].transform.position;
				renderer.transform.rotation = _spriteInputs[num].transform.rotation;
				renderer.transform.localScale = _spriteInputs[num].transform.lossyScale;
				renderer.flipX = _spriteInputs[num].flipX;
				renderer.sprite = _spriteInputs[num].sprite;
				renderer.material = _spriteInputs[num].material;
				renderer.DOKill();
				renderer.DOFade(startOpacity, 0f);
				renderer.DOFade(0f, _lifeTime).OnComplete(() =>
				{
					renderer.gameObject.SetActive(value: false);
				});
			}
			num++;
		}
		if (idx >= _trailLength - 1)
		{
			StopTrail();
		}
	}

	public bool IsTrailing()
	{
		return _isTrailing;
	}

	private void OnDestroy()
	{
		DOTween.Kill(this);
		if (_trailGroup != null)
		{
			Object.Destroy(_trailGroup);
		}
	}
}
