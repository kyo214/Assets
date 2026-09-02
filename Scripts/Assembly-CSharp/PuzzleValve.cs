using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleValve : MonoBehaviour, IPuzzle
{
	[SerializeField]
	private List<PipeObject> _pipeTypeList = new List<PipeObject>();

	[SerializeField]
	private string _animPlayName;

	[SerializeField]
	private string _animStopName;

	[SerializeField]
	private ItemInteractable interactableObject;

	[SerializeField]
	private Transform _valve;

	[SerializeField]
	private List<Transform> _needle = new List<Transform>();

	[SerializeField]
	private List<Transform> _needleSprite = new List<Transform>();

	[SerializeField]
	private List<Image> _meterGas = new List<Image>();

	[SerializeField]
	private PuzzleNetworkBehaviour _puzzleNetworkBehaviour;

	[SerializeField]
	private Transform _valveIngameObject;

	[SerializeField]
	private Sprite _spriteMeterGasOn;

	[SerializeField]
	private Sprite _spriteMeterGasOff;

	[SerializeField]
	private string _sfxNameValveRotatingUI;

	[SerializeField]
	private string _sfxNameValveRotatingObject;

	private void OnEnable()
	{
		_puzzleNetworkBehaviour.OnIdxChange += GasPipeChange;
	}

	private void OnDisable()
	{
		_puzzleNetworkBehaviour.OnIdxChange -= GasPipeChange;
	}

	private void Start()
	{
		for (int i = 0; i < _pipeTypeList.Count; i++)
		{
			_needle[i].DOKill(complete: true);
			if (_pipeTypeList[i].ColliderPipeList[0].activeSelf)
			{
				_needle[i].DOLocalRotate(new Vector3(0f, 0f, -100f), 0f);
				_valve.DOKill(complete: true);
				_valve.DOLocalRotate(new Vector3(0f, 0f, i * -90), 0.2f);
				_valveIngameObject.DOLocalRotate(new Vector3(_valveIngameObject.rotation.x, _valveIngameObject.rotation.y, 180 + i * 90), 0.2f);
				for (int j = 0; j < _pipeTypeList[i].ColliderPipeList.Count; j++)
				{
					_pipeTypeList[i].ColliderPipeList[j].SetActive(value: true);
					_pipeTypeList[i].AnimatorPipeList[j].Play(_animPlayName);
				}
				_meterGas[i].sprite = _spriteMeterGasOn;
			}
			else
			{
				_needle[i].DOLocalRotate(new Vector3(0f, 0f, 135f), 0f);
				for (int k = 0; k < _pipeTypeList[i].ColliderPipeList.Count; k++)
				{
					_pipeTypeList[i].ColliderPipeList[k].SetActive(value: false);
					_pipeTypeList[i].AnimatorPipeList[k].Play(_animStopName);
				}
				_meterGas[i].sprite = _spriteMeterGasOff;
			}
		}
	}

	public void InitAnswer()
	{
	}

	public void Show()
	{
		if (_puzzleNetworkBehaviour.isInitialized)
		{
			return;
		}
		for (int i = 0; i < _pipeTypeList.Count; i++)
		{
			if (_pipeTypeList[i].ColliderPipeList.Count > 0 && _pipeTypeList[i].ColliderPipeList[0].activeSelf)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					_puzzleNetworkBehaviour.isInitialized = true;
					_puzzleNetworkBehaviour.currentIdx = (byte)i;
				}
				else
				{
					_puzzleNetworkBehaviour.RPCChangeIdx((byte)i);
				}
				break;
			}
		}
	}

	public void ActivateGasPipe(bool isGoingLeft)
	{
		if (_sfxNameValveRotatingUI != "")
		{
			AudioManager.PlaySFX(_sfxNameValveRotatingUI);
		}
		int num = _puzzleNetworkBehaviour.currentIdx;
		if (isGoingLeft)
		{
			if (num > 0)
			{
				num--;
			}
		}
		else if (num < 2)
		{
			num++;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			_puzzleNetworkBehaviour.currentIdx = num;
		}
		else
		{
			_puzzleNetworkBehaviour.RPCChangeIdx(num);
		}
	}

	public void GasPipeChange(int index)
	{
		if (_sfxNameValveRotatingObject != "")
		{
			AudioManager.PlaySFXTransform(_sfxNameValveRotatingObject, _valveIngameObject.transform, isLocalPlayerTrigger: false);
		}
		for (int i = 0; i < _pipeTypeList.Count; i++)
		{
			if (i != index)
			{
				for (int j = 0; j < _pipeTypeList[i].ColliderPipeList.Count; j++)
				{
					_pipeTypeList[i].AnimatorPipeList[j].Play(_animStopName);
				}
				_meterGas[i].sprite = _spriteMeterGasOff;
				_needle[i].DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f).SetDelay(0.2f);
				_needle[i].DOLocalRotate(new Vector3(0f, 0f, 135f), 0.1f).SetDelay(0.3f);
				continue;
			}
			for (int k = 0; k < _pipeTypeList[i].ColliderPipeList.Count; k++)
			{
				_pipeTypeList[i].AnimatorPipeList[k].Play(_animPlayName);
			}
			_needle[i].DOLocalRotate(new Vector3(0f, 0f, -100f), 0.2f, RotateMode.FastBeyond360).SetDelay(0.2f);
			_meterGas[i].sprite = _spriteMeterGasOn;
			_valveIngameObject.DOLocalRotate(new Vector3(_valveIngameObject.rotation.x, _valveIngameObject.rotation.y, 180 + i * 90), 0.2f);
			_valve.DOLocalRotate(new Vector3(0f, 0f, i * -90), 0.2f);
		}
	}

	public void Hide()
	{
	}

	public void FixedUpdate()
	{
		if (!interactableObject || !interactableObject.UIMenu || interactableObject.UIMenu.isHidden)
		{
			return;
		}
		for (int i = 0; i < _pipeTypeList.Count; i++)
		{
			if (i == _puzzleNetworkBehaviour.currentIdx)
			{
				_needleSprite[i].localRotation = Quaternion.Euler(0f, 0f, Random.Range(-2f, 2f));
			}
		}
	}

	public void Navigate(Vector2 direction)
	{
		if (direction.x < 0f)
		{
			ActivateGasPipe(isGoingLeft: true);
		}
		else if (direction.x > 0f)
		{
			ActivateGasPipe(isGoingLeft: false);
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		yield return new WaitForSeconds(0.2f);
		interactableObject.UIMenu.Hide();
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
		}
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		UIGameManager.Instance.mapUI.SetActive(value: true);
		UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: true);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
	}

	public void Action1Press()
	{
	}

	public void Action1Release()
	{
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		interactableObject = intObject;
	}

	public ItemInteractable GetInteractableObject()
	{
		return interactableObject;
	}

	public void SetPassword(string pass)
	{
	}
}
