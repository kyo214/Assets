using System;
using System.Collections;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleElectricBox : MonoBehaviour, IPuzzle
{
	[Header("Object References")]
	[SerializeField]
	private string _resourcePath;

	public Sprite[] LedSpriteMap;

	public Sprite MissingPanelSprite;

	public Sprite SlotPanelSprite;

	[SerializeField]
	private Sprite _ledOn;

	[SerializeField]
	private Sprite _ledOff;

	[Header("Transform References")]
	[SerializeField]
	private Transform _piecesHolder;

	[SerializeField]
	private Image[] _leds;

	[SerializeField]
	private Transform[] _fuses;

	[Header("Generation Settings")]
	[SerializeField]
	private int _testSeed;

	[SerializeField]
	private int _itemId;

	private InventoryObject _checkItem;

	private bool _fuseInstalled;

	private int _seed;

	private bool _success;

	[SerializeField]
	private GameObject[] _puzzleSetRefs;

	private PuzzleSetElectricBox _puzzleSet;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		_fuseInstalled = true;
		StartCoroutine(GetSeed());
		GeneratePuzzle();
	}

	private void ResetPuzzle()
	{
		GeneratePuzzle();
	}

	public IEnumerator GetSeed()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
	}

	private void GeneratePuzzle()
	{
		if (_puzzleSetRefs.Length != 0)
		{
			UnityEngine.Random.InitState(_seed);
			int num = UnityEngine.Random.Range(0, _puzzleSetRefs.Length);
			GameObject gameObject = UnityEngine.Object.Instantiate(_puzzleSetRefs[num], _piecesHolder);
			gameObject.transform.localPosition = new Vector3(32f, 0f, 0f);
			_puzzleSet = gameObject.GetComponent<PuzzleSetElectricBox>();
			_puzzleSet.SetupInteractives(base.transform.GetComponent<PuzzleElectricBox>(), _seed);
			UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		}
	}

	public void Action1Press()
	{
		if (_fuseInstalled)
		{
			_puzzleSet.Action1Press();
		}
	}

	public bool GetFuseState()
	{
		return _fuseInstalled;
	}

	public void Action1Release()
	{
	}

	public ItemInteractable GetInteractableObject()
	{
		return _interactableTrigger;
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		_puzzleSet.Navigate(direction);
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		_interactableTrigger = intObject;
	}

	public void SetPassword(string pass)
	{
	}

	public void Show()
	{
		StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		_puzzleSet.Show();
		bool active = false;
		Sprite sprite = _ledOff;
		if (_fuseInstalled)
		{
			active = true;
			sprite = _ledOn;
		}
		else
		{
			_checkItem = NetworkGameManager.Instance.ownPlayer.data.FindInventory(_itemId);
			if (_checkItem != null)
			{
				yield return new WaitForSeconds(0.75f);
				_fuseInstalled = true;
				active = true;
				sprite = _ledOn;
			}
		}
		Transform[] fuses = _fuses;
		for (int i = 0; i < fuses.Length; i++)
		{
			fuses[i].gameObject.SetActive(active);
		}
		Image[] leds = _leds;
		for (int i = 0; i < leds.Length; i++)
		{
			leds[i].sprite = sprite;
		}
		yield return new WaitForSeconds(0f);
	}

	public void Hide()
	{
	}

	public bool GetSuccess()
	{
		return _success;
	}

	public void TriggerSuccess()
	{
		_success = true;
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator PuzzleUnlocked()
	{
		_success = true;
		yield return new WaitForSeconds(0.2f);
		AudioManager.PlaySFX("electric-box-beep-twice");
		yield return new WaitForSeconds(0.5f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID, triggerOnReverse: false, isForceInteract: true);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}
}
